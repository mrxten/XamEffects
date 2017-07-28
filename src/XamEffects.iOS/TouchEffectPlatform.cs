using System;
using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEffects;
using XamEffects.iOS;
using XamEffects.iOS.GestureCollectors;

[assembly: ResolutionGroupName("XamEffects")]
[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]
namespace XamEffects.iOS
{
    public class TouchEffectPlatform : PlatformEffect
    {
        private UIView _view;
        private UIView _layer;
        private double _alpha;

        protected override void OnAttached()
        {
            _view = Control ?? Container;

            _view.UserInteractionEnabled = true;

            TapGestureCollector.Add(_view, TapAction);
            LongTapGestureCollector.Add(_view, LongTapAction);

            UpdateEffectColor();
        }

        protected override void OnDetached()
        {
            TapGestureCollector.Delete(_view, TapAction);
            LongTapGestureCollector.Delete(_view, LongTapAction);
            _layer?.Dispose();
            _layer = null;
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == TouchEffect.ColorProperty.PropertyName)
            {
                UpdateEffectColor();
            }
        }

        private async void LongTapAction(UIGestureRecognizerState state)
        {
            switch (state)
            {
                case UIGestureRecognizerState.Began:
                    await TapAnimation(0.5, 0, _alpha, false);
                    break;
                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    await TapAnimation(0.5, _alpha);
                    break;
            }
        }

        private async void TapAction()
        {
            await TapAnimation(0.3, _alpha, 0);
        }

        private void UpdateEffectColor()
        {
            _layer?.Dispose();
            _layer = null;

            var color = TouchEffect.GetColor(Element);
            if (color == Color.Default)
            {
                return;
            }
            _alpha = color.A < 1.0 ? 1 : 0.3;

            _layer = new UIView {BackgroundColor = color.ToUIColor()};
        }

        private async Task TapAnimation(double duration, double start = 1, double end = 0, bool remove = true)
        {
            if (_layer != null)
            {
                _layer.Frame = new CGRect(0, 0, Container.Bounds.Width, Container.Bounds.Height);
                Container.AddSubview(_layer);
                Container.BringSubviewToFront(_layer);
                _layer.Alpha = (float)start;
                await UIView.AnimateAsync(duration, () => {
                    _layer.Alpha = (float)end;
                });
                if (remove)
                {
                    _layer?.RemoveFromSuperview();
                }
            }
        }
    }
}