using System;
using System.Threading.Tasks;
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

        protected override void OnAttached()
        {
            _view = Control ?? Container;

            _view.UserInteractionEnabled = true;

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
            Commands.GetTap(Element)?.Execute(Commands.GetTapParameter(Element));
        }

        private async void LongTapAction(UIGestureRecognizerState state)
        {
            switch (state)
            {
                case UIGestureRecognizerState.Began:
                    await Task.Delay(500);
                    Commands.GetLongTap(Element)?.Execute(Commands.GetLongTapParameter(Element));
                    break;
                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    break;
            }
        }
    }
}