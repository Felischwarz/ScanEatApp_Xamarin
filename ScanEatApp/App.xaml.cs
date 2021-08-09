using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ScanEatApp
{
    public partial class App : Application
    {
        public App()
        {
            //Device.SetFlags(new string[] { "AppTheme_Experimental" });
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
