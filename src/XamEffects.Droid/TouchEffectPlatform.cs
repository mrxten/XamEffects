using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamEffects;
using XamEffects.Droid;
using XamEffects.Droid.Collectors;
using Color = Xamarin.Forms.Color;
using View = Android.Views.View;

[assembly: ResolutionGroupName("XamEffects")]
[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]
namespace XamEffects.Droid
{
    public class TouchEffectPlatform : PlatformEffect
    {
        public bool EnableRipple => Build.VERSION.SdkInt <= BuildVersionCodes.Lollipop;

        public bool IsFastRenderers = global::Xamarin.Forms.Forms.Flags.Any(x => x == "FastRenderers_Experimental");

        private DateTime _tapTime;
        private View _view;
        private Android.Graphics.Color _color;
        private RippleDrawable _ripple;
        private FrameLayout _viewOverlay;
        private Rect _rect;
        private bool _touchEndInside;

        protected override void OnAttached()
        {
            _view = Control ?? Container;

            if (Control is Android.Widget.ListView)
            {
                //Except ListView because of Raising Exception OnClick
                return;
            }

            _viewOverlay = ViewOverlayCollector.Add(Container, this);

            if (EnableRipple)
                AddRipple();
            else
                _viewOverlay.Touch += OnTouch;

            UpdateEffectColor();
        }
        
        protected override void OnDetached()
        {
            var renderer = Container as IVisualElementRenderer;
            if (renderer?.Element != null) // Check disposed
            {
                _viewOverlay.Touch -= OnTouch;

                ViewOverlayCollector.Delete(Container, this);

                if (EnableRipple)
                    RemoveRipple();
            }
        }

        private void OnTouch(object sender, View.TouchEventArgs args)
        {
            switch (args.Event.Action)
            {
                case MotionEventActions.Down:
                    _tapTime = DateTime.Now;
                    _rect = new Rect(_viewOverlay.Left, _viewOverlay.Top, _viewOverlay.Right, _viewOverlay.Bottom);
                    TapAnimation(250, 0, 80);
                    break;
                case MotionEventActions.Move:
                    _touchEndInside = _rect.Contains(_viewOverlay.Left + (int) args.Event.GetX(),
                        _viewOverlay.Top + (int) args.Event.GetY());
                    break;
                case MotionEventActions.Up:
                    if (_touchEndInside)
                        if ((DateTime.Now - _tapTime).Milliseconds > 1500)
                            _viewOverlay.PerformLongClick();
                        else
                            _viewOverlay.CallOnClick();
                    
                    goto case MotionEventActions.Cancel;
                case MotionEventActions.Cancel:
                    args.Handled = false;
                    TapAnimation(250, 80);
                    break;
            }
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == TouchEffect.ColorProperty.PropertyName)
            {
                UpdateEffectColor();
            }
        }

        private void UpdateEffectColor()
        {
            var color = TouchEffect.GetColor(Element);
            if (color == Color.Default)
            {
                return;
            }
            _color = color.ToAndroid();
            _color.A = 80;

            if (EnableRipple)
            {
                _ripple.SetColor(GetPressedColorSelector(_color));
            }
        }

        private void AddRipple()
        {
            var color = TouchEffect.GetColor(Element);
            if (color == Color.Default)
            {
                return;
            }
            _color = color.ToAndroid();
            _color.A = 80;

            _viewOverlay.Foreground = CreateRipple(Color.Accent.ToAndroid());
            _ripple.SetColor(GetPressedColorSelector(_color));
        }

        private void RemoveRipple()
        {
            _viewOverlay.Foreground = null;
            _ripple?.Dispose();
            _ripple = null;
        }

        private RippleDrawable CreateRipple(Android.Graphics.Color color)
        {
            if (Element is Layout)
            {
                var mask = new ColorDrawable(Android.Graphics.Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }

            var back = _view.Background;
            if (back == null)
            {
                var mask = new ColorDrawable(Android.Graphics.Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }
            else if (back is RippleDrawable)
            {
                _ripple = (RippleDrawable) back.GetConstantState().NewDrawable();
                _ripple.SetColor(GetPressedColorSelector(color));

                return _ripple;
            }
            else
            {
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
            }
        }

        private static ColorStateList GetPressedColorSelector(int pressedColor)
        {
            return new ColorStateList(
                new[]
                {
                    new int[]{}
                },
                new[]
                {
                    pressedColor,
                });
        }

        private void TapAnimation(long duration, byte startAlpha = 255, byte endAlpha = 0)
        {
            var start = _color;
            var end = _color;
            start.A = startAlpha;
            end.A = endAlpha;
            var animation = ObjectAnimator.OfObject(_viewOverlay, "BackgroundColor", new ArgbEvaluator(), start.ToArgb(), end.ToArgb());
            animation.SetDuration(duration);
            animation.RepeatCount = 0;
            animation.RepeatMode = ValueAnimatorRepeatMode.Restart;
            animation.Start();
            animation.AnimationEnd += AnimationOnAnimationEnd;
        }

        private void AnimationOnAnimationEnd(object sender, EventArgs eventArgs)
        {
            var anim = ((ObjectAnimator) sender);
            anim.AnimationEnd -= AnimationOnAnimationEnd;
            anim.Dispose();
        }
    }
}
