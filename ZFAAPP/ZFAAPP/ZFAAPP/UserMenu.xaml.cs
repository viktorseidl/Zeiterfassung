using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZFAAPP.Models;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserMenu : ContentPage
    {
        public User user { get; set; }
        public UserMenu(User benutzer)
        {
            InitializeComponent();
            user = benutzer;
        }

        private void MyDataSettings(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MyData(user));
        }

        
        private void SecuritySettings(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MySafety(user));
        }

        private void ApplicationSettings(object sender, EventArgs e)
        {

        }

        private void SupportSettings(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MySupport(user));
        }

        private async void BackHomeScreen(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Home(user));
        }
    }
}