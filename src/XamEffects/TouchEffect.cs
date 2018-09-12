﻿using System.Linq;
using Xamarin.Forms;

namespace XamEffects
{
    public static class TouchEffect
    {
        public static void Init()
        {
            // for linker
        }

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
            if (!(bindable is View view))
                return;

            var eff = view.Effects.FirstOrDefault(e => e is TouchRoutingEffect);
            if (GetColor(bindable) != Color.Default)
            {
	            view.InputTransparent = false;

                if (eff == null) 
                {
                    view.Effects.Add(new TouchRoutingEffect());
                    if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout && !EffectsConfig.GetChildrenInputTransparent(view)) 
                    {
                        EffectsConfig.SetChildrenInputTransparent(view, true);
                    }
                }
            }
            else
            {
                if (eff != null && view.BindingContext != null) 
                {
                    view.Effects.Remove(eff);
                    if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout && EffectsConfig.GetChildrenInputTransparent(view)) {
                        EffectsConfig.SetChildrenInputTransparent(view, false);
                    }
                }
            }
        }
    }

    public class TouchRoutingEffect : RoutingEffect
    {
        public TouchRoutingEffect() : base("XamEffects." + nameof(TouchEffect)) { }
    }
}
