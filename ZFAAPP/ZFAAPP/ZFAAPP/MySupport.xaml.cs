using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZFAAPP.Models;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MySupport : ContentPage
    {
        public User user { get; set; }
        public string Reason { get; set; }

        public string Description { get; set; }
        public MySupport(User benutzer)
        {
            InitializeComponent();
            user = benutzer;
        }

        private void BtnBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void BtnSendSupport(object sender, EventArgs e)
        {
            Description = SupportDescription.Text;
            if (String.IsNullOrEmpty(Description) == false && String.IsNullOrEmpty(Reason) == false && String.IsNullOrEmpty(MMail.Text) == false)
            {
                SupportTicket supportTicket = new SupportTicket
                {
                    Mid = this.user.Id,
                    Mvname = this.user.V_name,
                    Mnname = this.user.N_name,
                    SReason = Reason,
                    Mail = MMail.Text,
                    SDescription = Description
                };
                var url = $"https://itsnando.com/api/api/insert/supportTicketAll.php";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
                var client = new HttpClient(clientHandler);
                var json = JsonConvert.SerializeObject(supportTicket);
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
                        //User can be logged in
                        var data = jObj["message"].ToString();
                        await DisplayAlert("Information", data.ToString(), "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Fehler", "Ein Fehler ist aufgetreten. Bitte versuchen Sie es erneut.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Fehler", "Ein Fehler ist aufgetreten. Bitte versuchen Sie es erneut.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Hinweis", "Bitte füllen Sie alle Felder aus!", "OK");
            }
        }

        private void SaveInVar(object sender, EventArgs e)
        {
            var item = sender as Picker;
            if (item != null)
            {
                Reason = item.SelectedItem.ToString();
            }
        }
    }
}