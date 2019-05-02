//thanks to Andrei Nitescu (https://github.com/andreinitescu/Xamore/blob/master/Xamore.Controls.Droid/Renderers/BorderRendererVisual.cs)

using System;
using Android.Graphics.Drawables;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using System.Linq;
using Android.Graphics;

namespace XamEffects.Droid.Renderers {
    public static class BorderRendererVisual {
        public static void UpdateBackground(BorderView touchView, Android.Views.View view) {
            var borderWidth = touchView.BorderWidth;
            var context = view.Context;

            GradientDrawable strokeDrawable = null;

            if (borderWidth > 0) {
                strokeDrawable = new GradientDrawable();
                strokeDrawable.SetColor(touchView.BackgroundColor.ToAndroid());

                strokeDrawable.SetStroke((int)context.ToPixels(borderWidth), touchView.BorderColor.ToAndroid());
                strokeDrawable.SetCornerRadius(context.ToPixels(touchView.CornerRadius));
            }

            var backgroundDrawable = new GradientDrawable();
            backgroundDrawable.SetColor(touchView.BackgroundColor.ToAndroid());
            backgroundDrawable.SetCornerRadius(context.ToPixels(touchView.CornerRadius));

            if (strokeDrawable != null) {
                var ld = new LayerDrawable(new Drawable[] { strokeDrawable, backgroundDrawable });
                ld.SetLayerInset(1, (int)context.ToPixels(borderWidth), (int)context.ToPixels(borderWidth), (int)context.ToPixels(borderWidth), (int)context.ToPixels(borderWidth));
                view.SetBackgroundDrawable(ld);
            }
            else {
                view.SetBackgroundDrawable(backgroundDrawable);
            }

            view.SetPadding(
                (int)context.ToPixels(borderWidth + touchView.Padding.Left),
                (int)context.ToPixels(borderWidth + touchView.Padding.Top),
                (int)context.ToPixels(borderWidth + touchView.Padding.Right),
                (int)context.ToPixels(borderWidth + touchView.Padding.Bottom));
        }

        static double ThickestSide(this Thickness t) {
            return new double[] {
                t.Left,
                t.Top,
                t.Right,
                t.Bottom
            }.Max();
        }

        public static void SetClipPath(this BorderViewRenderer renderer, Canvas canvas) {
            var clipPath = new Path();
            var radius = renderer.Context.ToPixels(renderer.Element.CornerRadius) - renderer.Context.ToPixels((float)renderer.Element.Padding.ThickestSide());

            var w = renderer.Width;
            var h = renderer.Height;

            clipPath.AddRoundRect(new RectF(
                renderer.ViewGroup.PaddingLeft,
                renderer.ViewGroup.PaddingTop,
                w - renderer.ViewGroup.PaddingRight,
                h - renderer.ViewGroup.PaddingBottom),
                radius,
                radius,
                Path.Direction.Cw);

            canvas.ClipPath(clipPath);
        }
    }
}
