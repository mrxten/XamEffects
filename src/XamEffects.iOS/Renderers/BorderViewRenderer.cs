using System;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEffects;
using XamEffects.Helpers;
using XamEffects.iOS.GestureCollectors;
using XamEffects.iOS.GestureRecognizers;
using XamEffects.iOS.Renderers;

[assembly: ExportRenderer(typeof(BorderView), typeof(BorderViewRenderer))]

namespace XamEffects.iOS.Renderers {
    public class BorderViewRenderer : VisualElementRenderer<BorderView> {
        public static void Link() {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BorderView> e) {
            base.OnElementChanged(e);
            
            if (e.NewElement == null) return;
            NativeView.ClipsToBounds = true;
            NativeView.Layer.AllowsEdgeAntialiasing = true;
            NativeView.Layer.EdgeAntialiasingMask = CAEdgeAntialiasingMask.All;
            SetCornerRadius();
            SetBorders();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == BorderView.CornerRadiusProperty.PropertyName)
                SetCornerRadius();
            else if (
                e.PropertyName == BorderView.BorderWidthProperty.PropertyName ||
                e.PropertyName == BorderView.BorderColorProperty.PropertyName)
                SetBorders();
        }

        #region Borders

        void SetCornerRadius() {
            NativeView.Layer.CornerRadius = new nfloat(Element.CornerRadius);
        }

        void SetBorders() {
            NativeView.Layer.BorderWidth = new nfloat(Element.BorderWidth);
            NativeView.Layer.BorderColor = Element.BorderColor.ToCGColor();
        }

        #endregion
    }
}