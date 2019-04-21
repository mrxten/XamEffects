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
using XamEffects.iOS.GestureRecognizers;
using System;
using System.Linq;
using Foundation;

[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]

namespace XamEffects.iOS {
    public class TouchEffectPlatform : PlatformEffect {
        public bool IsDisposed => (Container as IVisualElementRenderer)?.Element == null;
        public UIView View => Control ?? Container;

        UIView _layer;
        nfloat _alpha;
        CancellationTokenSource _cancellation;

        protected override void OnAttached() {
            View.UserInteractionEnabled = true;
            _layer = new UIView {
                UserInteractionEnabled = false,
                Opaque = false
            };

            UpdateEffectColor();
            TouchGestureCollector.Add(View, OnTouch);
        }

        protected override void OnDetached() {
            TouchGestureCollector.Delete(View, OnTouch);
            _layer?.RemoveFromSuperview();
            _layer?.Dispose();
        }

        void OnTouch(TouchGestureRecognizer.TouchArgs e) {
            switch (e.State) {
                case TouchGestureRecognizer.TouchState.Started:
                    BringLayer();
                    break;

                case TouchGestureRecognizer.TouchState.Ended:
                    EndAnimation();
                    break;

                case TouchGestureRecognizer.TouchState.Cancelled:
                    if (!IsDisposed && _layer != null) {
                        _cancellation?.Cancel();
                        _layer.Alpha = 0;
                        _layer.RemoveFromSuperview();
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
            if (color == Color.Default) {
                return;
            }

            _alpha = color.A < 1.0 ? 1 : (nfloat)0.3;
            _layer.BackgroundColor = color.ToUIColor();
        }

        void BringLayer() {
            _cancellation?.Cancel();
            _layer.Alpha = _alpha;
            _layer.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
            View.AddSubview(_layer);
            View.BringSubviewToFront(_layer);
        }

        async Task EndAnimation() {
            if (!IsDisposed && _layer != null) {
                _cancellation?.Cancel();
                _cancellation = new CancellationTokenSource();
                var token = _cancellation.Token;

                await UIView.AnimateAsync(0.225,
                () => {
                    if (!token.IsCancellationRequested && !IsDisposed)
                        _layer.Alpha = 0;
                });

                if (!IsDisposed && !token.IsCancellationRequested) {
                    _layer?.RemoveFromSuperview();
                }
            }
        }

        public static void Init() {
        }
    }
}