using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEffects;
using XamEffects.iOS;

[assembly: ResolutionGroupName("XamEffects")]
[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]
namespace XamEffects.iOS
{
    public class TouchEffectPlatform : PlatformEffect
    {
        private UITapGestureRecognizer _tapGesture;
        private UILongPressGestureRecognizer _longTapGesture;
        private UIView _view;
        private UIView _layer;
        private double _alpha;

        protected override void OnAttached()
        {
            _view = Control ?? Container;
            UpdateEffectColor();

            _tapGesture = new UITapGestureRecognizer(async (obj) => {
                await TapAnimation(0.3, _alpha, 0);
            });

            _longTapGesture = new UILongPressGestureRecognizer(async (obj) => {
                switch (obj.State)
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
            });

            _view.AddGestureRecognizer(_longTapGesture);
            _view.AddGestureRecognizer(_tapGesture);
        }

        protected override void OnDetached()
        {
            _view.RemoveGestureRecognizer(_tapGesture);
            _tapGesture.Dispose();

            _view.RemoveGestureRecognizer(_longTapGesture);
            _longTapGesture.Dispose();

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

        private void UpdateEffectColor()
        {
            _layer?.Dispose();
            _layer = null;

            var color = TouchEffect.GetColor(Element);
            if (color == Color.Default)
            {
                return;
            }
            _alpha = color.A < 1.0 ? 1 : 0.25;

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
                    _layer.RemoveFromSuperview();
                }
            }
        }
    }
}