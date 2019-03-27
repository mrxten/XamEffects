using System.ComponentModel;
using System.Windows.Input;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEffects;
using XamEffects.iOS;
using XamEffects.iOS.GestureCollectors;

[assembly: ExportEffect(typeof(CommandsPlatform), nameof(Commands))]

namespace XamEffects.iOS {
    public class CommandsPlatform : PlatformEffect {
        public UIView View => Control ?? Container;

        ICommand _tapCommand;
        ICommand _longCommand;
        object _tapParameter;
        object _longParameter;

        protected override void OnAttached() {
            View.UserInteractionEnabled = true;

            UpdateTap();
            UpdateTapParameter();
            UpdateLongTap();
            UpdateLongTapParameter();

            TapGestureCollector.Add(View, TapAction);
            LongTapGestureCollector.Add(View, LongTapAction);
        }

        protected override void OnDetached() {
            TapGestureCollector.Delete(View, TapAction);
            LongTapGestureCollector.Delete(View, LongTapAction);
        }

        void TapAction() {
            if (_tapCommand?.CanExecute(_tapParameter) ?? false)
                _tapCommand.Execute(_tapParameter);
        }

        void LongTapAction(UIGestureRecognizerState state, bool inside) {
            switch (state) {
                case UIGestureRecognizerState.Began:
                    break;
                case UIGestureRecognizerState.Ended:
                    if (!inside) return;
                    if (_longCommand == null)
                        TapAction();
                    else if (_longCommand?.CanExecute(_longParameter) ?? false)
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
        }

        void UpdateTapParameter() {
            _tapParameter = Commands.GetTapParameter(Element);
        }

        void UpdateLongTap() {
            _longCommand = Commands.GetLongTap(Element);
        }

        void UpdateLongTapParameter() {
            _longParameter = Commands.GetLongTapParameter(Element);
        }

        public static void Init() {
        }
    }
}