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
    public partial class MySafety : ContentPage
    {
        public User user { get; set; }
        public MySafety(User benutzer)
        {
            InitializeComponent();
            user = benutzer;
        }

        private void BtnGenerateQR(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ShowMyQRCode(user));
        }

        private void GetQRCode(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ShowMyQRCode(user));
        }

        private void ChangeMyPass(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PassChange(user));
        }

        private void BtnBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}