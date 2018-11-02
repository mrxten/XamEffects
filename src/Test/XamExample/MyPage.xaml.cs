using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace XamExample {
    public partial class MyPage : ContentPage {
        public MyPage() {
            InitializeComponent();

            var c = 0;

            XamEffects.TouchEffect.SetColor(plus, Color.White);
            XamEffects.Commands.SetTap(plus, new Command(() => {
                c++;
                counter.Text = $"Touches: {c}";
            }));

            XamEffects.TouchEffect.SetColor(minus, Color.White);
            XamEffects.Commands.SetLongTap(minus, new Command(() => {
                c--;
                counter.Text = $"Touches: {c}";
            }));
        }
    }
}
