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
        public bool EnableRipple => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;

        public bool IsDisposed => ((IVisualElementRenderer)Container).Element == null;

        private View _view;
        private Android.Graphics.Color _color;
        private RippleDrawable _ripple;
        private FrameLayout _viewOverlay;
	    private bool _rippleOnScreen;

		public static void Init()
	    {
		    
	    }

        protected override void OnAttached()
        {
            _view = Control ?? Container;

			if (Control is Android.Widget.ListView || Control is Android.Widget.ScrollView)
            {
                //Except ListView and ScrollView because of Raising Exception OnClick
                return;
            }

	        _view.Clickable = true;
	        _view.LongClickable = true;

			_viewOverlay = new FrameLayout(Container.Context)
	        {
		        LayoutParameters = new ViewGroup.LayoutParams(-1, -1)
	        };
	        Container.LayoutChange += ViewOnLayoutChange;

			if (EnableRipple)
                AddRipple();
            
			_view.Touch += OnTouch;

            UpdateEffectColor();
        }
        
        protected override void OnDetached()
        {
            if (IsDisposed)
            {
                if (EnableRipple)
                    RemoveRipple();
	            _view.Touch -= OnTouch;

	            ViewOverlayCollector.TryDelete(Container, this);
			}
		}

        private void OnTouch(object sender, View.TouchEventArgs args)
        {
            switch (args.Event.Action)
            {
                case MotionEventActions.Down:
	                if (EnableRipple)
		                ForceStartRipple(args.Event.GetX(), args.Event.GetY());
	                else
	                {
		                _rippleOnScreen = true;
						TapAnimation(250, 0, 80);
					}
					break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    args.Handled = false;
	                if (EnableRipple)
		                ForceEndRipple();
	                else
	                {
		                _rippleOnScreen = false;
		                TapAnimation(250, 80);
					}
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

            _viewOverlay.Background = CreateRipple(color.ToAndroid());
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
            if (IsDisposed)
                return;
            
	        if (_viewOverlay.Parent == null)
				Container.AddView(_viewOverlay);
	        _viewOverlay.BringToFront();

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
            if (IsDisposed)
                return;
            
			if (!_rippleOnScreen)
				Container.RemoveView(_viewOverlay);
			var anim = ((ObjectAnimator) sender);
            anim.AnimationEnd -= AnimationOnAnimationEnd;
            anim.Dispose();
		}

		private void ForceStartRipple(float x, float y)
	    {
		    if (_viewOverlay.Background is RippleDrawable bc)
		    {
			    _rippleOnScreen = true;
				if (_viewOverlay.Parent == null)
					Container.AddView(_viewOverlay);
			    _viewOverlay.BringToFront();
				bc.SetHotspot(x, y);

			    Task.Run(async () =>
			    {
				    await Task.Delay(25);
				    Device.BeginInvokeOnMainThread(() =>
				    {
					    _viewOverlay.Pressed = true;
				    });
			    });
		    }
		}

	    private void ForceEndRipple()
	    {
		    _rippleOnScreen = false;
			_viewOverlay.Pressed = false;
		    Task.Run(async () =>
		    {
			    await Task.Delay(250);
				if(!_rippleOnScreen)
					Device.BeginInvokeOnMainThread(() =>
					{
                        if (IsDisposed)
                            return;
						Container.RemoveView(_viewOverlay);
					});
			});
	    }

	    private void ViewOnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs)
	    {
		    var group = ((ViewGroup)sender);
			_viewOverlay.Right = group.Width;
		    _viewOverlay.Bottom = group.Height;
	    }
	}
}
