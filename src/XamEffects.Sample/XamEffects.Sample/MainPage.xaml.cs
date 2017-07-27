using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamEffects.Sample
{
    public partial class MainPage : ContentPage
    {
        public ICommand TapCommand { get; }

        public ICommand LongTapCommand { get; }

        public MainPage()
        {
            TapCommand = new Command((() =>
            {
                DisplayAlert("Alert", "Item tapped", "Ok");
            }));

            LongTapCommand = new Command((() =>
            {
                DisplayAlert("Alert", "Item long tapped", "Ok");
            }));

            InitializeComponent();
            BindingContext = this;
        }
    }
}
