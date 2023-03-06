using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Security.Cryptography;
using ZFAAPP.Models;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Newtonsoft.Json.Linq;

namespace ZFAAPP
{
    public partial class MainPage : ContentPage
    {
        private string Pass;
        private string Pin;
        public User user { get; set; }
        public MainPage(User benutzer)
        {
            InitializeComponent();
            this.user = benutzer;
            Pass =PPass.Text;
            Pin=PPin.Text;            
        }

        private void BtnForgotData(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ForgotLog(user));
        }

        private void BtnQrScanner(object sender, EventArgs e)
        {
            Navigation.PushAsync(new QRScanner(user));
        }

        private void BtnChangePass(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PassChange(user));
        }
        private async void SendToHome()
        {
            await Navigation.PushAsync(new Home(user));
        }
        private async void BtnLoginZFA(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(PPass.Text)==false && String.IsNullOrEmpty(PPin.Text)==false) {
                var mySHA256 = new MD5CryptoServiceProvider();
                var PassWord = BitConverter.ToString(mySHA256.ComputeHash(Encoding.UTF8.GetBytes(PPass.Text))).Replace("-", string.Empty).ToLower();
                var PinWord = BitConverter.ToString(mySHA256.ComputeHash(Encoding.UTF8.GetBytes(PPin.Text))).Replace("-", string.Empty).ToLower();
                var GKey = BitConverter.ToString(mySHA256.ComputeHash(Encoding.UTF8.GetBytes(PassWord+PinWord))).Replace("-", string.Empty).ToLower();
                //Console.WriteLine(PassWord);
                //Console.WriteLine(PinWord);
                //Console.WriteLine(GKey);
                Login log = new Login
                {
                    PIN = PPass.Text,
                    TID = PPin.Text
                };                
                var url = $"https://itsnando.com/api/api/select/loginAll.php";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
                var client = new HttpClient(clientHandler);
                var json = JsonConvert.SerializeObject(log);
                Console.WriteLine(json.ToString());
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, contentJson);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    string cjson = content.ToString();
                    Console.WriteLine(cjson);
                    var jObj = JObject.Parse(cjson);

                    if (jObj.ContainsKey("data"))
                    {
                        //User can be logged in
                        var data = jObj["data"].ToString();
                        var uObj = JArray.Parse(data);
                        user.Id = (int)uObj[0]["ID"];
                        user.V_name = (string)uObj[0]["Name2"];
                        user.N_name = (string)uObj[0]["Name1"];
                        user.TimeTouchNr = Int32.Parse(log.TID);
                        user.LogState = true;
                        user.SetKeysOnUser(PassWord.ToString(), PinWord.ToString());
                        await Navigation.PushAsync(new Home(user));                        
                    }
                    else
                    {
                        //Error on Database Connection or wrong query
                        var data = jObj["message"].ToString();
                        await DisplayAlert("Anmeldung fehlerhaft", data, "OK");
                    }
                    
                }
        
            }

        }
        
    }
}
