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
    public partial class ForgotTel : ContentPage
    {
        public User user { get; set; }
        public ForgotTel(User benutzer)
        {
            InitializeComponent();
            this.user = benutzer;
        }

        private void btnBackHome(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(user));
        }
    }
}