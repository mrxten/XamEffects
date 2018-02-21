using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
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

	    private DateTime _tapTime;
	    private readonly Rect _rect = new Rect();
	    private readonly int[] _location = new int[2];

		public static void Init()
	    {
		    
	    }

        protected override void OnAttached()
        {
            _view = Control ?? Container;
	        _view.Clickable = true;
	        _view.LongClickable = true;
            _view.Click += ViewOnClick;
	        _view.LongClick += ViewOnLongClick;

			_view.Touch += ViewOnTouch;
        }

	    private void ViewOnTouch(object sender, View.TouchEventArgs args)
	    {
			switch (args.Event.Action)
			{
				case MotionEventActions.Down:
					_tapTime = DateTime.Now;
					break;
				case MotionEventActions.Up:
					if (IsViewInBounds((int)args.Event.RawX, (int)args.Event.RawY))
						if ((DateTime.Now - _tapTime).Milliseconds > 1500)
							_view.PerformLongClick();
						else
							_view.CallOnClick();

					goto case MotionEventActions.Cancel;
				case MotionEventActions.Cancel:
					args.Handled = false;
					break;
			}
		}

	    private bool IsViewInBounds(int x, int y)
	    {
		    _view.GetDrawingRect(_rect);
		    _view.GetLocationOnScreen(_location);
		    _rect.Offset(_location[0], _location[1]);
		    return _rect.Contains(x, y);
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
	            _view.Click -= ViewOnClick;
	            _view.LongClick -= ViewOnLongClick;
	            _view.Touch -= ViewOnTouch;
			}
		}
    }
}