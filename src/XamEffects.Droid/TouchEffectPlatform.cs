using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Animation;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamEffects;
using XamEffects.Droid;
using Color = Android.Graphics.Color;
using ListView = Android.Widget.ListView;
using ScrollView = Android.Widget.ScrollView;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]

namespace XamEffects.Droid {
    public class TouchEffectPlatform : PlatformEffect {
        public bool EnableRipple => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
        public bool IsDisposed => (Container as IVisualElementRenderer)?.Element == null;
        public View View => Control ?? Container;

        Color _color;
        byte _alpha;
        RippleDrawable _ripple;
        FrameLayout _viewOverlay;
        bool _rippleOnScreen;
        ObjectAnimator _animator;

        public static void Init() {
        }

        protected override void OnAttached() {
            if (Control is ListView || Control is ScrollView) {
                return;
            }

            View.Clickable = true;
            View.LongClickable = true;
            View.Touch += OnTouch;

            _viewOverlay = new FrameLayout(Container.Context) {
                LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
                Clickable = false,
                Focusable = false,
            };
            Container.LayoutChange += ViewOnLayoutChange;

            if (EnableRipple)
                _viewOverlay.Background = CreateRipple(_color);

            SetEffectColor();
        }

        protected override void OnDetached() {
            if (!IsDisposed) return;
            if (EnableRipple) {
                _viewOverlay.Foreground = null;
                _viewOverlay.Dispose();
                _ripple?.Dispose();
            }
            View.Touch -= OnTouch;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == TouchEffect.ColorProperty.PropertyName) {
                SetEffectColor();
            }
        }

        void SetEffectColor() {
            var color = TouchEffect.GetColor(Element);
            if (color == Xamarin.Forms.Color.Default) {
                return;
            }

            _color = color.ToAndroid();
            _alpha = _color.A == 255 ? (byte)80 : _color.A;

            if (EnableRipple) {
                _ripple.SetColor(GetPressedColorSelector(_color));
            }
        }

        void OnTouch(object sender, View.TouchEventArgs args) {
            switch (args.Event.Action) {
                case MotionEventActions.Down:
                    _rippleOnScreen = true;

                    if (EnableRipple)
                        ForceStartRipple(args.Event.GetX(), args.Event.GetY());
                    else
                        TapAnimation(125, 0, _alpha);

                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    args.Handled = false;
                    _rippleOnScreen = false;

                    if (EnableRipple)
                        ForceEndRipple();
                    else
                        TapAnimation(250, _alpha, 0);

                    break;
            }
        }

        void ViewOnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs) {
            var group = (ViewGroup)sender;
            if (group == null) return;
            _viewOverlay.Right = group.Width;
            _viewOverlay.Bottom = group.Height;
        }

        #region Ripple

        RippleDrawable CreateRipple(Color color) {
            if (Element is Layout) {
                var mask = new ColorDrawable(Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }

            var back = View.Background;
            if (back == null) {
                var mask = new ColorDrawable(Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }

            if (back is RippleDrawable) {
                _ripple = (RippleDrawable)back.GetConstantState().NewDrawable();
                _ripple.SetColor(GetPressedColorSelector(color));

                return _ripple;
            }

            return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
        }

        static ColorStateList GetPressedColorSelector(int pressedColor) {
            return new ColorStateList(
                new[] { new int[] { } },
                new[] { pressedColor, });
        }

        async void ForceStartRipple(float x, float y) {
            if (IsDisposed)
                return;

            if (!(_viewOverlay.Background is RippleDrawable bc)) return;
            if (_viewOverlay.Parent == null)
                Container.AddView(_viewOverlay);
            _viewOverlay.BringToFront();
            bc.SetHotspot(x, y);

            await Task.Delay(25);
            Device.BeginInvokeOnMainThread(() => {
                if (!IsDisposed)
                    _viewOverlay.Pressed = true;
            });
        }

        async void ForceEndRipple() {
            if (IsDisposed)
                return;

            _viewOverlay.Pressed = false;
            await Task.Delay(250);
            if (!_rippleOnScreen && !IsDisposed)
                Device.BeginInvokeOnMainThread(() => {
                    Container.RemoveView(_viewOverlay);
                });
        }

        #endregion

        #region Overlay

        void TapAnimation(long duration, byte startAlpha, byte endAlpha) {
            if (IsDisposed)
                return;

            if (_viewOverlay.Parent == null)
                Container.AddView(_viewOverlay);
            _viewOverlay.BringToFront();

            var start = _color;
            var end = _color;
            start.A = startAlpha;
            end.A = endAlpha;

            ClearAnimation();
            _animator = ObjectAnimator.OfObject(_viewOverlay,
                "BackgroundColor",
                new ArgbEvaluator(),
                start.ToArgb(),
                end.ToArgb());
            _animator.SetDuration(duration);
            _animator.RepeatCount = 0;
            _animator.RepeatMode = ValueAnimatorRepeatMode.Restart;
            _animator.Start();
            _animator.AnimationEnd += AnimationOnAnimationEnd;
        }

        void AnimationOnAnimationEnd(object sender, EventArgs eventArgs) {
            if (IsDisposed)
                return;

            if (!_rippleOnScreen && !IsDisposed)
                Container.RemoveView(_viewOverlay);
            ClearAnimation();
        }

        void ClearAnimation() {
            if (_animator != null) {
                _animator.AnimationEnd -= AnimationOnAnimationEnd;
                _animator.Cancel();
                _animator.Dispose();
                _animator = null;
            }
        }

        #endregion
    }
}