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
        TouchGestureRecognizer _touchRecognizer;
        CancellationTokenSource _cancellation;

        protected override void OnAttached() {
            _touchRecognizer = new TouchGestureRecognizer {
                Delegate = new TouchGestureRecognizerDelegate(View)
            };

            View.AddGestureRecognizer(_touchRecognizer);
            View.UserInteractionEnabled = true;

            _layer = new UIView {
                UserInteractionEnabled = false,
                Opaque = false
            };

            UpdateEffectColor();
            _touchRecognizer.OnTouch += TouchRecognizer_OnTouch;
        }

        protected override void OnDetached() {
            _touchRecognizer.OnTouch -= TouchRecognizer_OnTouch;
            View.RemoveGestureRecognizer(_touchRecognizer);

            _layer?.RemoveFromSuperview();
            _layer?.Dispose();
        }

        async void TouchRecognizer_OnTouch(object sender, TouchGestureRecognizer.TouchArgs e) {
            switch (e.State) {
                case TouchGestureRecognizer.TouchState.Started:
                    await TapAnimation(0.1, 0, _alpha, false);
                    break;

                case TouchGestureRecognizer.TouchState.Ended:
                    await TapAnimation(0.25, _alpha, 0, true);
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

        async Task TapAnimation(double duration, nfloat start, nfloat end, bool remove) {
            if (!IsDisposed && _layer != null) {
                _cancellation?.Cancel();
                _cancellation = new CancellationTokenSource();
                var token = _cancellation.Token;

                _layer.RemoveFromSuperview();
                View.AddSubview(_layer);
                View.BringSubviewToFront(_layer);
                _layer.Alpha = start;
                _layer.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
                await UIView.AnimateAsync(duration,
                    () => {
                        if (!token.IsCancellationRequested && !IsDisposed)
                            _layer.Alpha = end;
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