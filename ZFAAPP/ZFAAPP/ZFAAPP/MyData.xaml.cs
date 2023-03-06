using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZFAAPP.Models;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyData : ContentPage
    {
        public User user { get; set; }
        public MyData(User benutzer)
        {
            InitializeComponent();
            user = benutzer;
            Vorname.Text = user.V_name;
            M_ID.Text = user.Id.ToString();
            Nachname.Text = user.N_name;
        }

        private async void BtnSaveMyData(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Nachname.Text) == false)
            {
                MeineDaten meineDaten = new MeineDaten
                {
                    Name1 = Nachname.Text,
                    ID = user.Id
                };
                var url = $"https://itsnando.com/api/api/update/mitarbeiter/updateMydata.php";
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
                var client = new HttpClient(clientHandler);
                var json = JsonConvert.SerializeObject(meineDaten);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, contentJson);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    string cjson = content.ToString();
                    Console.WriteLine(cjson);
                    var jObj = JObject.Parse(cjson);

                    if (jObj.ContainsKey("message"))
                    {
                        var data = jObj["message"].ToString();
                        if (data == "True")
                        {
                            user.N_name = Nachname.Text;
                            await DisplayAlert(user.V_name.ToString(), "Ihre Daten wurden aktualisiert!", "OK");
                            await Navigation.PopAsync();
                            

                        }
                        else
                        {
                            await DisplayAlert(user.V_name.ToString(), "Ihre Daten konnten nicht aktualisiert werden!", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert(user.V_name.ToString(), "Ihre Daten konnten nicht aktualisiert werden!", "OK");
                    }
                }
                else
                {
                    await DisplayAlert(user.V_name.ToString(), "Ihre Daten konnten nicht aktualisiert werden!", "OK");
                }
            }
        }

        private void BtnBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}