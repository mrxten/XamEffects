using System.Windows.Input;
using Xamarin.Forms;
using XamEffects.Helpers;

namespace XamEffects.Controls {
    public class BorderView : ContentView {
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(
                nameof(CornerRadius),
                typeof(float),
                typeof(BorderView),
                default(float));

        public float CornerRadius {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly BindableProperty BorderWidthProperty =
            BindableProperty.Create(
                nameof(BorderWidth),
                typeof(double),
                typeof(BorderView),
                default(double));

        public double BorderWidth {
            get => (double)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(
                nameof(BorderColor),
                typeof(Color),
                typeof(BorderView),
                Color.Default);

        public Color BorderColor {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }
    }
}