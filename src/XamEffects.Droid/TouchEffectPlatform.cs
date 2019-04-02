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
        RippleDrawable _ripple;
        FrameLayout _viewOverlay;
        bool _rippleOnScreen;
        ObjectAnimator _animator;

        public static void Init() {
        }

        protected override void OnAttached() {
            if (Control is ListView || Control is ScrollView) {
                //Except ListView and ScrollView because of Raising Exception OnClick
                return;
            }

            View.Clickable = true;
            View.LongClickable = true;

            _viewOverlay = new FrameLayout(Container.Context) {
                LayoutParameters = new ViewGroup.LayoutParams(-1, -1), Clickable = false, Focusable = false,
            };
            Container.LayoutChange += ViewOnLayoutChange;

            if (EnableRipple)
                AddRipple();

            View.Touch += OnTouch;
            UpdateEffectColor();
        }

        protected override void OnDetached() {
            if (!IsDisposed) return;
            if (EnableRipple)
                RemoveRipple();
            View.Touch -= OnTouch;
        }

        void OnTouch(object sender, View.TouchEventArgs args) {
            switch (args.Event.Action) {
                case MotionEventActions.Down:
                    if (EnableRipple)
                        ForceStartRipple(args.Event.GetX(), args.Event.GetY());
                    else {
                        _rippleOnScreen = true;
                        TapAnimation(250, 0, 80);
                    }

                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    args.Handled = false;
                    if (EnableRipple)
                        ForceEndRipple();
                    else {
                        _rippleOnScreen = false;
                        TapAnimation(250, 80);
                    }

                    break;
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == TouchEffect.ColorProperty.PropertyName) {
                UpdateEffectColor();
            }
        }

        void UpdateEffectColor() {
            var color = TouchEffect.GetColor(Element);
            if (color == Xamarin.Forms.Color.Default) {
                return;
            }

            _color = color.ToAndroid();
            _color.A = 80;

            if (EnableRipple) {
                _ripple.SetColor(GetPressedColorSelector(_color));
            }
        }

        void AddRipple() {
            var color = TouchEffect.GetColor(Element);
            if (color == Xamarin.Forms.Color.Default) {
                return;
            }

            _color = color.ToAndroid();
            _color.A = 80;

            _viewOverlay.Background = CreateRipple(color.ToAndroid());
        }

        void RemoveRipple() {
            _viewOverlay.Foreground = null;
            _ripple?.Dispose();
            _ripple = null;
        }

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
                _ripple = (RippleDrawable) back.GetConstantState().NewDrawable();
                _ripple.SetColor(GetPressedColorSelector(color));

                return _ripple;
            }

            return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
        }

        static ColorStateList GetPressedColorSelector(int pressedColor) {
            return new ColorStateList(
                new[] {new int[] { }},
                new[] {pressedColor,});
        }

        void TapAnimation(long duration, byte startAlpha = 255, byte endAlpha = 0) {
            if (IsDisposed)
                return;

            if (_viewOverlay.Parent == null)
                Container.AddView(_viewOverlay);
            _viewOverlay.BringToFront();

            var start = _color;
            var end = _color;
            start.A = startAlpha;
            end.A = endAlpha;

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
            try {
                _animator.AnimationEnd -= AnimationOnAnimationEnd;
                _animator.Dispose();
            }
            catch {
            }
        }

        void ForceStartRipple(float x, float y) {
            if (IsDisposed)
                return;

            if (!(_viewOverlay.Background is RippleDrawable bc)) return;
            _rippleOnScreen = true;
            if (_viewOverlay.Parent == null)
                Container.AddView(_viewOverlay);
            _viewOverlay.BringToFront();
            bc.SetHotspot(x, y);

            Task.Run(async () => {
                await Task.Delay(25);
                Device.BeginInvokeOnMainThread(() => {
                    if (IsDisposed)
                        return;
                    _viewOverlay.Pressed = true;
                });
            });
        }

        void ForceEndRipple() {
            if (IsDisposed)
                return;

            _rippleOnScreen = false;
            _viewOverlay.Pressed = false;
            Task.Run(async () => {
                await Task.Delay(250);
                if (!_rippleOnScreen)
                    Device.BeginInvokeOnMainThread(() => {
                        if (IsDisposed)
                            return;
                        Container.RemoveView(_viewOverlay);
                    });
            });
        }

        void ViewOnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs) {
            var group = ((ViewGroup) sender);
            if (group == null) return;
            _viewOverlay.Right = group.Width;
            _viewOverlay.Bottom = group.Height;
        }
    }
}
