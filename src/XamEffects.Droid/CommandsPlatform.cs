using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamEffects;
using XamEffects.Droid;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(CommandsPlatform), nameof(Commands))]
namespace XamEffects.Droid
{
    public class CommandsPlatform : PlatformEffect
    {
        private Android.Views.View _view;

        protected override void OnAttached()
        {
            _view = Control ?? Container;

            _view.Click += ViewOnClick;
            _view.LongClick += ViewOnLongClick;
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            Commands.GetTap(Element)?.Execute(Commands.GetTapParameter(Element));
        }

        private void ViewOnLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var cmd = Commands.GetLongTap(Element);

            if (cmd == null)
            {
                longClickEventArgs.Handled = false;
                return;
            }
            
            cmd.Execute(Commands.GetLongTapParameter(Element));
            longClickEventArgs.Handled = true;
        }

        protected override void OnDetached()
        {
            _view.Click -= ViewOnClick;
            _view.LongClick -= ViewOnLongClick;
            _view = null;
        }
    }
}