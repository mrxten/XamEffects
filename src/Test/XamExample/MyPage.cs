using System;

using Xamarin.Forms;

namespace XamExample {
    public class MyPage : ContentPage {
        public MyPage() {
            var grid = new Grid {
                HeightRequest = 100,
                WidthRequest = 200,
                BackgroundColor = Color.Gray,
                Children = {
                    new Label() {
                        Text = $"Click",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    }
                }
            };

            var c = 0;
            var label = new Label() {
                Text = $"Touches: {c}"
            };

            XamEffects.TouchEffect.SetColor(grid, Color.White);
            XamEffects.Commands.SetTap(grid, new Command(() => {
                c++;
                label.Text = $"Touches: {c}";
            }));
            XamEffects.EffectsConfig.SetChildrenInputTransparent(grid, true);


            Content = new StackLayout {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children = {
                    grid,
                    label
                }
            };

        }
    }
}

