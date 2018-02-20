using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using XamEffects.Droid.Collectors;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(CommandsPlatform), nameof(Commands))]
namespace XamEffects.Droid
{
    public class CommandsPlatform : PlatformEffect
    {
        private Android.Views.View _view;

        private FrameLayout _clickOverlay;

	    public static void Init()
	    {
		    
	    }

        protected override void OnAttached()
        {
            _view = Control ?? Container;
            _clickOverlay = ViewOverlayCollector.AddOrGet(Container, this);
            _clickOverlay.Click += ViewOnClick;
            _clickOverlay.LongClick += ViewOnLongClick;
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
            var renderer = Container as IVisualElementRenderer;
            if (renderer?.Element != null) // Check disposed
            {
                _clickOverlay.Click -= ViewOnClick;
                _clickOverlay.LongClick -= ViewOnLongClick;

                ViewOverlayCollector.TryDelete(Container, this);
            }
        }
    }
}