using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZFAAPP.Models;
using static System.Net.WebRequestMethods;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowMyQRCode : ContentPage
    {
        public User user { get; set; }
        public ShowMyQRCode(User benutzer)
        {
            InitializeComponent();
            user = benutzer;
            var mySHA256 = new MD5CryptoServiceProvider();

            MyQRCodeImage.BarcodeValue = (string)BitConverter.ToString(mySHA256.ComputeHash(Encoding.UTF8.GetBytes(user.GetHashTid().ToString()+user.GetHashPass().ToString()))).Replace("-", string.Empty).ToLower();
            Console.WriteLine(user.GetHashTid().ToString());
            Console.WriteLine(user.GetHashPass().ToString());
        }

        private async void BtnTakeScreenshot(object sender, EventArgs e)
        {
            if (Screenshot.IsCaptureSupported)
            {
                var screenshot = await Screenshot.CaptureAsync();
                var stream = await screenshot.OpenReadAsync();
                var i = ImageSource.FromStream(()=> stream);
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                documentsPath = Path.Combine(documentsPath, "Share");
                Directory.CreateDirectory(documentsPath);
                string filePath = Path.Combine(documentsPath, "share.png");
                byte[] bArray = new byte[stream.Length];
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    using (stream)
                    {
                        stream.Read(bArray, 0, (int)stream.Length);
                    }
                    int length = bArray.Length;
                    fs.Write(bArray, 0, length);
                }
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = Title,
                    File = new ShareFile(filePath)
                });
            }
        }
    }
}