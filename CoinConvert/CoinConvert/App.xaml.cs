using System;
using CoinConvert.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoinConvert
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            ((MainViewModel) MainPage.BindingContext).SaveAllData();
        }

        protected override void OnResume()
        {
        }
    }
}