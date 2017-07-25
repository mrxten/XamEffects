using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Util;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XF.SelectionEffect;
using XF.SelectionEffect.Droid;
using View = Android.Views.View;

[assembly: ResolutionGroupName("SelectionEffect")]
[assembly: ExportEffect(typeof(AddEffectPlatform), nameof(AddEffect))]
namespace XF.SelectionEffect.Droid
{
    public class AddEffectPlatform : PlatformEffect
    {
        private Android.Views.View _view;
        private FrameLayout _layer;
        private Android.Graphics.Color _color;
        private RippleDrawable _ripple;

        protected override void OnAttached()
        {
            _view = Control ?? Container;

            if (_view is Android.Widget.ListView)
            {
                //Except ListView because of Raising Exception OnClick
                return;
            }
            _view.Touch += OnTouch;

            UpdateEffectColor();
        }

        protected override void OnDetached()
        {
            _ripple?.Dispose();
            _ripple = null;
            _view.Touch -= OnTouch;
        }

        private void OnTouch(object sender, View.TouchEventArgs args)
        {
            if (args.Event.Action == MotionEventActions.Down)
            {
                Container.AddView(_layer);
                _layer.Top = 0;
                _layer.Left = 0;
                _layer.Right = _view.Width;
                _layer.Bottom = _view.Height;
                _layer.BringToFront();
            }
            if (args.Event.Action == MotionEventActions.Up || args.Event.Action == MotionEventActions.Cancel)
            {
                Container.RemoveView(_layer);
            }
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == AddEffect.ColorProperty.PropertyName)
            {
                UpdateEffectColor();
            }
        }

        private void UpdateEffectColor()
        {
            _view.Touch -= OnTouch;
            if (_layer != null)
            {
                _layer.Dispose();
                _layer = null;
            }

            var color = AddEffect.GetColor(Element);
            if (color == Color.Default)
            {
                return;
            }
            _color = color.ToAndroid();
            _color.A = 75;

            _layer = new FrameLayout(Container.Context);
            _layer.LayoutParameters = new ViewGroup.LayoutParams(-1, -1);
            _layer.SetBackgroundColor(_color);
            _view.Touch += OnTouch;
        }
    }
}
