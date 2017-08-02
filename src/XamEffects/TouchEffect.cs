using System.Linq;
using Xamarin.Forms;

namespace XamEffects
{
    public static class TouchEffect
    {
        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached(
                "Color",
                typeof(Color),
                typeof(TouchEffect),
                Color.Default,
                propertyChanged: PropertyChanged
            );

        public static void SetColor(BindableObject view, Color value)
        {
            view.SetValue(ColorProperty, value);
        }

        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
                return;


            var eff = view.Effects.FirstOrDefault(e => e is TouchRoutingEffect);
            if (GetColor(bindable) != Color.Default)
            {
                if (eff == null)
                    view.Effects.Add(new TouchRoutingEffect());
            }
            else
            {
                if (eff != null && view.BindingContext != null)
                    view.Effects.Remove(eff);
            }
        }
    }

    public class TouchRoutingEffect : RoutingEffect
    {
        public TouchRoutingEffect() : base("XamEffects." + nameof(TouchEffect)) { }
    }
}
