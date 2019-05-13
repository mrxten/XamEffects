using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamEffects;
using XamEffects.Droid.Renderers;
using AColor = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(BorderView), typeof(BorderViewRenderer))]
namespace XamEffects.Droid.Renderers {
    public class BorderViewRenderer : VisualElementRenderer<BorderView> {
        public static void Init() {
        }

        public BorderViewRenderer(Context context) : base(context) {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BorderView> e) {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            BorderRendererVisual.UpdateBackground(Element, this);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == BorderView.BorderColorProperty.PropertyName ||
                e.PropertyName == BorderView.BorderWidthProperty.PropertyName ||
                e.PropertyName == BorderView.CornerRadiusProperty.PropertyName ||
                e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName) {
                BorderRendererVisual.UpdateBackground(Element, this);
            }
        }

        protected override void DispatchDraw(Canvas canvas) {
            canvas.Save(SaveFlags.Clip);
            BorderRendererVisual.SetClipPath(this, canvas);
            base.DispatchDraw(canvas);
            canvas.Restore();
        }
    }
}