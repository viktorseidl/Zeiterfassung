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
    public partial class HolidayRequest : ContentPage
    {
        public User user { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string Atype { get; set; }
        public HolidayRequest(User benutzer)
        {
            InitializeComponent();
            user = benutzer;
        }

        private void BtnBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnDateSelectedfrom(object sender, DateChangedEventArgs e)
        {
            FromDate = e.NewDate.Date.ToString("dd-MM-yyyy");
        }
        private void OnDateSelectedto(object sender, DateChangedEventArgs e)
        {
            ToDate = e.NewDate.Date.ToString("dd-MM-yyyy");
        }

        private async void Beantragen(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(FromDate) == false 
                && String.IsNullOrEmpty(ToDate) == false
                && String.IsNullOrEmpty(UTage.Text) == false
                && String.IsNullOrEmpty(Atype) == false
                && String.IsNullOrEmpty(ToDate) == false)
            {
                HolidayAntrag holidayAntrag = new HolidayAntrag
                {
                    MID= user.Id,
                    Mvname= user.V_name,
                    Mnname= user.N_name,
                    Datum = FromDate,
                    BisDatum = ToDate,
                    Antragtyp = Atype,
                    Bemerkung = Description.Text,
                    AnzahlTage = UTage.Text
                };
                var url = $"https://itsnando.com/api/api/update/requestHolidays.php";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
                var client = new HttpClient(clientHandler);
                var json = JsonConvert.SerializeObject(holidayAntrag);
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

        private void SelectionType(object sender, EventArgs e)
        {
            var item = sender as Picker;
            if (item != null)
            {
                Atype = item.SelectedItem.ToString();
            }
        }
    }
}