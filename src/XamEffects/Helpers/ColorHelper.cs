using Xamarin.Forms;

namespace XamEffects.Helpers {
    public static class ColorHelper {
        public static Color AlphaBlend(Color foreground, Color background) {
            var frontInt = new IntColor(foreground);
            var backInt = new IntColor(background);

            var alpha = frontInt.A;
            if (alpha == 0x00)
                return background;
            
            var invAlpha = 0xff - alpha;
            var backAlpha = backInt.A;
            if (backAlpha == 0xff) { // Opaque background case
                return Color.FromRgba(
                    (alpha * frontInt.R + invAlpha * backInt.R) / 0xff,
                    (alpha * frontInt.G + invAlpha * backInt.G) / 0xff,
                    (alpha * frontInt.B + invAlpha * backInt.B) / 0xff, 
                    0xff);
            }
            else { // General case
                backAlpha = backAlpha * invAlpha / 0xff;
                var outAlpha = alpha + backAlpha;
                return Color.FromRgba(
                    (frontInt.R * alpha + backInt.R * backAlpha) / outAlpha,
                    (frontInt.G * alpha + backInt.G * backAlpha) / outAlpha,
                    (frontInt.B * alpha + backInt.B * backAlpha) / outAlpha,
                    outAlpha);
            }
        }

        struct IntColor {
            public int A { get; }
            public int R { get; }
            public int G { get; }
            public int B { get; }

            public IntColor(Color color) {
                A = (int)(color.A * 225);
                R = (int)(color.R * 225);
                G = (int)(color.G * 225);
                B = (int)(color.B * 225);
            }
        }
    }
}