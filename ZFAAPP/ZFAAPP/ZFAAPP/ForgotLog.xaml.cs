using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZFAAPP.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotLog : ContentPage
    {
        public User user { get; set; }
        public ForgotLog(User benutzer)
        {
            InitializeComponent();
            this.user = benutzer;
        }

        private void btnForgotDMail(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ForgotEmail(user));
        }

        private void btnForgotDTel(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ForgotTel(user));
        }

        private void btnBackHome(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(user));
        }
    }
}