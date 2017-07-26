using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamEffects
{
    public static class Commands
    {
        public static readonly BindableProperty OnProperty =
            BindableProperty.CreateAttached(
                "On",
                typeof(bool),
                typeof(Commands),
                false,
                propertyChanged: OnOffChanged
            );

        public static void SetOn(BindableObject view, bool value)
        {
            view.SetValue(OnProperty, value);
        }

        public static bool GetOn(BindableObject view)
        {
            return (bool)view.GetValue(OnProperty);
        }

        public static readonly BindableProperty TapProperty =
            BindableProperty.CreateAttached(
                "Tap",
                typeof(ICommand),
                typeof(Commands),
                default(ICommand)
            );

        public static void SetTap(BindableObject view, ICommand value)
        {
            view.SetValue(TapProperty, value);
        }

        public static ICommand GetTap(BindableObject view)
        {
            return (ICommand)view.GetValue(TapProperty);
        }

        public static readonly BindableProperty TapParameterProperty =
            BindableProperty.CreateAttached(
                "TapParameter",
                typeof(object),
                typeof(Commands),
                default(object)
            );

        public static void SetTapParameter(BindableObject view, object value)
        {
            view.SetValue(TapParameterProperty, value);
        }

        public static object GetTapParameter(BindableObject view)
        {
            return view.GetValue(TapParameterProperty);
        }

        public static readonly BindableProperty LongTapProperty =
            BindableProperty.CreateAttached(
                "LongTap",
                typeof(ICommand),
                typeof(Commands),
                default(ICommand)
            );

        public static void SetLongTap(BindableObject view, ICommand value)
        {
            view.SetValue(LongTapProperty, value);
        }

        public static ICommand GetLongTap(BindableObject view)
        {
            return (ICommand)view.GetValue(LongTapProperty);
        }

        public static readonly BindableProperty LongTapParameterProperty =
            BindableProperty.CreateAttached(
                "LongTapParameter",
                typeof(object),
                typeof(Commands),
                default(object)
            );

        public static void SetLongTapParameter(BindableObject view, object value)
        {
            view.SetValue(LongTapParameterProperty, value);
        }

        public static object GetLongTapParameter(BindableObject view)
        {
            return view.GetValue(LongTapParameterProperty);
        }

        private static void OnOffChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
                return;

            if ((bool)newValue)
            {
                view.Effects.Add(new CommandsRoutingEffect());
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is CommandsRoutingEffect);
                if (toRemove != null)
                    view.Effects.Remove(toRemove);
            }
        }
    }

    public class CommandsRoutingEffect : RoutingEffect
    {
        public CommandsRoutingEffect() : base("XamEffects." + nameof(Commands)) { }
    }
}
