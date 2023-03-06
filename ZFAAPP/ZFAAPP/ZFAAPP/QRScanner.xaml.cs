using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZFAAPP.Models;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class QRScanner : ContentPage
    {
        public User user { get; set; }
        public QRScanner(User benutzer)
        {
            InitializeComponent();
            this.user = benutzer;

         }
        //Result result
        private void Handle_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () => {
                //36d1a8545ee72e5c041408f37f927b3e
                if (String.IsNullOrEmpty(result.Text) == false)
                {
                    QRLog log = new QRLog
                    {
                        NGK = result.Text
                    };
                    var url = $"https://itsnando.com/api/api/select/loginAllQR.php";
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
                    var client = new HttpClient(clientHandler);
                    var json = JsonConvert.SerializeObject(log);
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
                            var data = jObj["data"].ToString();
                            var uObj = JArray.Parse(data);
                            var mySHA256 = new MD5CryptoServiceProvider();
                            var PassWord = BitConverter.ToString(mySHA256.ComputeHash(Encoding.UTF8.GetBytes((string)uObj[0]["Pin"]))).Replace("-", string.Empty).ToLower();
                            var PinWord = BitConverter.ToString(mySHA256.ComputeHash(Encoding.UTF8.GetBytes((string)uObj[0]["TimeTouchNr"]))).Replace("-", string.Empty).ToLower();
                            user.Id = (int)uObj[0]["ID"];
                            user.V_name = (string)uObj[0]["Name2"];
                            user.N_name = (string)uObj[0]["Name1"];
                            user.TimeTouchNr = (int)uObj[0]["TimeTouchNr"];
                            user.LogState = true;
                            user.SetKeysOnUser(PassWord.ToString(), PinWord.ToString());

                            await Navigation.PushAsync(new Home(user));
                        }
                        else
                        {
                            //Error on Database Connection or wrong query
                            var data = jObj["message"].ToString();
                            await DisplayAlert("Fehlermeldung", data, "OK");
                        }

                    }
                
                };
                    

                
            });
        }

        
    }
}