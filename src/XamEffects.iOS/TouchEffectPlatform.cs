using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEffects;
using XamEffects.iOS;
using XamEffects.iOS.GestureCollectors;

[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]

namespace XamEffects.iOS {
    public class TouchEffectPlatform : PlatformEffect {
        public bool IsDisposed => (Container as IVisualElementRenderer)?.Element == null;
        public UIView View => Control ?? Container;

        UIView _layer;
        double _alpha;
        CancellationTokenSource _cancellation;

        protected override void OnAttached() {
            View.UserInteractionEnabled = true;

            TapGestureCollector.Add(View, TapAction);
            LongTapGestureCollector.Add(View, LongTapAction);
            UpdateEffectColor();
        }

        protected override void OnDetached() {
            TapGestureCollector.Delete(View, TapAction);
            LongTapGestureCollector.Delete(View, LongTapAction);
            _layer?.Dispose();
            _layer = null;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == TouchEffect.ColorProperty.PropertyName) {
                UpdateEffectColor();
            }
        }

        async void LongTapAction(UIGestureRecognizerState state, bool inside) {
            switch (state) {
                case UIGestureRecognizerState.Began:
                    await TapAnimation(0.3, 0, _alpha, false);
                    break;
                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    await TapAnimation(0.3, _alpha);
                    break;
            }
        }

        async void TapAction() {
            await TapAnimation(0.2, _alpha, 0);
        }

        void UpdateEffectColor() {
            _layer?.Dispose();
            _layer = null;

            var color = TouchEffect.GetColor(Element);
            if (color == Color.Default) {
                return;
            }

            _alpha = color.A < 1.0 ? 1 : 0.3;
            _layer = new UIView {BackgroundColor = color.ToUIColor(), UserInteractionEnabled = false,};
        }

        async Task TapAnimation(double duration, double start = 1, double end = 0, bool remove = true) {
            if (!IsDisposed && _layer != null) {
                _cancellation?.Cancel();
                _cancellation = new CancellationTokenSource();

                var token = _cancellation.Token;

                _layer.Frame = new CGRect(0, 0, Container.Bounds.Width, Container.Bounds.Height);
                Container.AddSubview(_layer);
                Container.BringSubviewToFront(_layer);
                _layer.Alpha = (float) start;
                await UIView.AnimateAsync(duration,
                    () => {
                        if (!token.IsCancellationRequested && !IsDisposed)
                            _layer.Alpha = (float) end;
                    });
                if (remove && !IsDisposed && !token.IsCancellationRequested) {
                    _layer?.RemoveFromSuperview();
                }
            }
        }

        public static void Init() {
        }
    }
}