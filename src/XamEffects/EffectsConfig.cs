using System;
using Xamarin.Forms;

namespace XamEffects {
    public static class EffectsConfig {
        public static void Init() {
            // for linker
        }

        public static bool AutoChildrenInputTransparent { get; set; } = true;

        public static readonly BindableProperty ChildrenInputTransparentProperty =
            BindableProperty.CreateAttached(
                "ChildrenInputTransparent",
                typeof(bool),
                typeof(EffectsConfig),
                false,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ConfigureChildrenInputTransparent(bindable);
                }
            );

        public static void SetChildrenInputTransparent(BindableObject view, bool value) {
            view.SetValue(ChildrenInputTransparentProperty, value);
        }

        public static bool GetChildrenInputTransparent(BindableObject view) {
            return (bool)view.GetValue(ChildrenInputTransparentProperty);
        }

        private static void ConfigureChildrenInputTransparent(BindableObject bindable) {
            if (!(bindable is Layout layout))
                return;

            if (GetChildrenInputTransparent(bindable)) {
                foreach (var layoutChild in layout.Children)
                    AddInputTransparentToElement(layoutChild);
                layout.ChildAdded += Layout_ChildAdded;
            }
            else {
                layout.ChildAdded -= Layout_ChildAdded;
            }
        }

        private static void Layout_ChildAdded(object sender, ElementEventArgs e) {
            AddInputTransparentToElement(e.Element);
        }

        private static void AddInputTransparentToElement(BindableObject obj) {
            if (obj is View view && TouchEffect.GetColor(view) == Color.Default && Commands.GetTap(view) == null && Commands.GetLongTap(view) == null) {
                view.InputTransparent = true;
            }
        }
    }
}