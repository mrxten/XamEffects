using System;
using System.Collections.Generic;
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
using View = Android.Views.View;

[assembly: ResolutionGroupName("XamEffects")]
[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]
namespace XamEffects.Droid
{
    public class TouchEffectPlatform : PlatformEffect
    {
        public bool EnableRipple => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;

        private View _view;
        private FrameLayout _layer;
        private Android.Graphics.Color _color;
        private RippleDrawable _ripple;
        private Drawable _orgDrawable;
        private FrameLayout _rippleOverlay;
        private ContainerOnLayoutChangeListener _rippleListener;
        private bool _deleteLayer = false;
        private IList<IGestureRecognizer> _gestures;

        protected override void OnAttached()
        {
            _view = Control ?? Container;
            _gestures = (Element as Xamarin.Forms.View)?.GestureRecognizers;

            if (_view is Android.Widget.ListView)
            {
                //Except ListView because of Raising Exception OnClick
                return;
            }
            
            if (EnableRipple)
                AddRipple();
            else
                _view.Touch += OnTouch;

            UpdateEffectColor();
        }

        protected override void OnDetached()
        {
            if(EnableRipple)
                RemoveRipple();

            _view.Touch -= OnTouch;
            _view = null;
        }

        private void OnTouch(object sender, View.TouchEventArgs args)
        {
            args.Handled = false;

            switch (args.Event.Action)
            {
                case MotionEventActions.Down:
                    Container.RemoveView(_layer);
                    Container.AddView(_layer);
                    _layer.Top = 0;
                    _layer.Left = 0;
                    _layer.Right = _view.Width;
                    _layer.Bottom = _view.Height;
                    _layer.BringToFront();
                    TapAnimation(250, 0, 65, false);
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    TapAnimation(250, 65, 0);
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
            _view.Touch -= OnTouch;
            _layer?.Dispose();
            _layer = null;

            var color = TouchEffect.GetColor(Element);
            if (color == Color.Default)
            {
                return;
            }
            _color = color.ToAndroid();

            if (EnableRipple)
            {
                _ripple.SetColor(GetPressedColorSelector(_color));
            }
            else
            {
                _layer = new FrameLayout(Container.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -1)
                };
                _layer.SetBackgroundColor(_color);
                _view.Touch += OnTouch;
            }
        }

        private void AddRipple()
        {
            if (Element is Layout)
            {
                _rippleOverlay = new FrameLayout(Container.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -1)
                };

                _rippleListener = new ContainerOnLayoutChangeListener(_rippleOverlay);
                _view.AddOnLayoutChangeListener(_rippleListener);

                ((ViewGroup)_view).AddView(_rippleOverlay);

                _rippleOverlay.BringToFront();
                _rippleOverlay.Foreground = CreateRipple(Color.Accent.ToAndroid());
            }
            else
            {
                _orgDrawable = _view.Background;
                _view.Background = CreateRipple(Color.Accent.ToAndroid());
            }

            _ripple.SetColor(GetPressedColorSelector(_color));
        }

        private void RemoveRipple()
        {
            if (Element is Layout)
            {
                var viewgrp = (ViewGroup)_view;
                viewgrp?.RemoveOnLayoutChangeListener(_rippleListener);
                viewgrp?.RemoveView(_rippleOverlay);

                _rippleListener?.Dispose();
                _rippleListener = null;

                _rippleOverlay?.Dispose();
                _rippleOverlay = null;
            }
            else
            {
                _view.Background = _orgDrawable;
                _orgDrawable?.Dispose();
                _orgDrawable = null;
            }
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

        private void TapAnimation(long duration, byte startAlpha = 1, byte endAlpha = 0, bool remove = true)
        {
            _deleteLayer = remove;
            var start = _color;
            var end = _color;
            start.A = startAlpha;
            end.A = endAlpha;
            var animation = ObjectAnimator.OfObject(_layer, "BackgroundColor", new ArgbEvaluator(), start.ToArgb(), end.ToArgb());
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
            if (_deleteLayer)
            {
                Container.RemoveView(_layer);
            }
        }

        internal class ContainerOnLayoutChangeListener : Java.Lang.Object, View.IOnLayoutChangeListener
        {
            private readonly FrameLayout _layout;

            public ContainerOnLayoutChangeListener(FrameLayout layout)
            {
                _layout = layout;
            }

            public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
            {
                _layout.Right = v.Width;
                _layout.Bottom = v.Height;
            }
        }
    }
}
