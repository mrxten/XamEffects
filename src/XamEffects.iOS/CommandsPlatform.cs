using System.ComponentModel;
using System.Windows.Input;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEffects;
using XamEffects.iOS;
using XamEffects.iOS.GestureCollectors;
using XamEffects.iOS.GestureRecognizers;

[assembly: ExportEffect(typeof(CommandsPlatform), nameof(Commands))]

namespace XamEffects.iOS {
    public class CommandsPlatform : PlatformEffect {
        public UIView View => Control ?? Container;

        ICommand _tapCommand;
        ICommand _longCommand;
        object _tapParameter;
        object _longParameter;
        UITapGestureRecognizer _tapRecognizer;
        UILongPressGestureRecognizer _longTapRecognizer;

        protected override void OnAttached() {
            View.UserInteractionEnabled = true;
            _tapRecognizer = new UITapGestureRecognizer(TapAction) {
                Delegate = new TapGestureRecognizerDelegate(View)
            };
            _longTapRecognizer = new UILongPressGestureRecognizer(LongTapAction) {
                Delegate = new TapGestureRecognizerDelegate(View)
            };
            
            UpdateTap();
            UpdateTapParameter();
            UpdateLongTap();
            UpdateLongTapParameter();
        }

        protected override void OnDetached() {
            View.RemoveGestureRecognizer(_tapRecognizer);
            View.RemoveGestureRecognizer(_longTapRecognizer);
            _tapRecognizer.Dispose();
            _longTapRecognizer.Dispose();
        }

        void TapAction() {
            if (_tapCommand?.CanExecute(_tapParameter) ?? false)
                _tapCommand.Execute(_tapParameter);
        }

        void LongTapAction(UILongPressGestureRecognizer uiLongPressGestureRecognizer) {
            var coord = uiLongPressGestureRecognizer.LocationInView(uiLongPressGestureRecognizer.View);
            var inside = uiLongPressGestureRecognizer.View.PointInside(coord, null);

            switch (uiLongPressGestureRecognizer.State) {
                case UIGestureRecognizerState.Began:
                    break;
                case UIGestureRecognizerState.Ended:
                    if (!inside) return;
                    if (_longCommand == null)
                        TapAction();
                    else if (_longCommand.CanExecute(_longParameter))
                        _longCommand.Execute(_longParameter);
                    break;
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    break;
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args) {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Commands.TapProperty.PropertyName)
                UpdateTap();
            else if (args.PropertyName == Commands.TapParameterProperty.PropertyName)
                UpdateTapParameter();
            else if (args.PropertyName == Commands.LongTapProperty.PropertyName)
                UpdateLongTap();
            else if (args.PropertyName == Commands.LongTapParameterProperty.PropertyName)
                UpdateLongTapParameter();
        }

        void UpdateTap() {
            _tapCommand = Commands.GetTap(Element);
            if (_tapCommand == null)
                View.RemoveGestureRecognizer(_tapRecognizer);
            else 
                View.AddGestureRecognizer(_tapRecognizer);
        }

        void UpdateTapParameter() {
            _tapParameter = Commands.GetTapParameter(Element);
        }

        void UpdateLongTap() {
            _longCommand = Commands.GetLongTap(Element);
            if (_longCommand == null)
                View.RemoveGestureRecognizer(_longTapRecognizer);
            else 
                View.AddGestureRecognizer(_longTapRecognizer);
        }

        void UpdateLongTapParameter() {
            _longParameter = Commands.GetLongTapParameter(Element);
        }

        public static void Init() {
        }
        
        public class TapGestureRecognizerDelegate : UIGestureRecognizerDelegate {
            readonly UIView _view;

            public TapGestureRecognizerDelegate(UIView view) {
                _view = view;
            }

            public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch) {
                return touch.View == _view;
            }
        }
    }
}