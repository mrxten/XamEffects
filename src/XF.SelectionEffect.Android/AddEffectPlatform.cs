using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XF.SelectionEffect;
using XF.SelectionEffect.Android;

[assembly: ResolutionGroupName("SelectionEffect")]
[assembly: ExportEffect(typeof(AddEffectPlatform), nameof(AddEffect))]
namespace XF.SelectionEffect.Android
{
    public class AddEffectPlatform : PlatformEffect
    {
        private Android.Views.View _view;
        private FrameLayout _layer;
        private RippleDrawable _ripple;

        protected override void OnAttached()
        {
            _view = Control ?? Container;

            if (Control is Android.Widget.ListView)
            {
                //Except ListView because of Raising Exception OnClick
                return;
            }
        }

        protected override void OnDetached()
        {
        }
    }
}