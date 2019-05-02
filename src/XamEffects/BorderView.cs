using Xamarin.Forms;

namespace XamEffects {
    public class BorderView : ContentView {
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(
                nameof(CornerRadius),
                typeof(double),
                typeof(BorderView),
                default(double));

        public double CornerRadius {
            get => (double)GetValue(CornerRadiusProperty);
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