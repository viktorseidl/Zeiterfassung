using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZFAAPP.Models;

namespace ZFAAPP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            User benutzer = new User();
            MainPage = new NavigationPage(new MainPage(benutzer));
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
