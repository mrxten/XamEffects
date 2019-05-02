using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XamEffects;

namespace XamExample {
    public partial class MyPage : ContentPage {
        public MyPage() {
            InitializeComponent();

            var c = 0;
            Commands.SetTap(touch, new Command(() => {
                c++;
                text.Text = c.ToString();
            }));
        }
    }
}
