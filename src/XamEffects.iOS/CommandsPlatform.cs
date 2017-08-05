using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEffects;
using XamEffects.iOS;
using XamEffects.iOS.GestureCollectors;

[assembly: ExportEffect(typeof(CommandsPlatform), nameof(Commands))]
namespace XamEffects.iOS
{
    public class CommandsPlatform : PlatformEffect
    {
        private UIView _view;

        private ICommand _tapCommand;

        private ICommand _longCommand;

        private object _tapParameter;

        private object _longParameter;

        protected override void OnAttached()
        {
            _view = Control ?? Container;
            _view.UserInteractionEnabled = true;

            UpdateTap();
            UpdateTapParameter();
            UpdateLongTap();
            UpdateLongTapParameter();

            TapGestureCollector.Add(_view, TapAction);
            LongTapGestureCollector.Add(_view, LongTapAction);
        }

        protected override void OnDetached()
        {
            TapGestureCollector.Delete(_view, TapAction);
            LongTapGestureCollector.Delete(_view, LongTapAction);
        }

        private void TapAction()
        {
           _tapCommand?.Execute(_tapParameter);
        }

        private async void LongTapAction(UIGestureRecognizerState state)
        {
            switch (state)
            {
                case UIGestureRecognizerState.Began:
                    await Task.Delay(500);
                    _longCommand?.Execute(_longParameter);
                    break;
                case UIGestureRecognizerState.Ended:
                    if (_longCommand == null)
                        TapAction();
                    break;
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    break;
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
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

        private void UpdateTap()
        {
            _tapCommand = Commands.GetTap(Element);
        }

        private void UpdateTapParameter()
        {
            _tapParameter = Commands.GetTapParameter(Element);
        }

        private void UpdateLongTap()
        {
            _longCommand = Commands.GetLongTap(Element);
        }

        private void UpdateLongTapParameter()
        {
            _longParameter = Commands.GetLongTapParameter(Element);
        }


        public static void Init()
        {
            
        }
    }
}