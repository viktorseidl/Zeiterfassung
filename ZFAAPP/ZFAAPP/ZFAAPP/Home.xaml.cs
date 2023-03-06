using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Home : ContentPage
    {
        public User user { get; set; }
        
        public IList<Zeitbuchungen> Zeitbuchungens { get; private set; }

        public Home(User benutzer)
        {
            
            this.user = benutzer;
            if(user== null || (((bool)user.LogState) == false))
            {
                Navigation.PushAsync(new MainPage(user));
            }
            else
            {
                InitializeComponent();
                LastBuchungen();
            }
            Welcome.Text = user.N_name; 
        }

        private void BtnUserMenu(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserMenu(user));
        }

        private void Logout(object sender, EventArgs e)
        {
            user = null;
            User benutzer = new User();
            benutzer.LogState = false;
            Navigation.PopAsync();
            App.Current.MainPage = new NavigationPage(new MainPage(benutzer));
                        }

        private async void LastBuchungen()
        {

            Zeitbuchungens = new List<Zeitbuchungen>();
            


            LastTimeEntries lastTimeEntries = new LastTimeEntries
            {
                MID=user.Id
            };
            var url = $"https://itsnando.com/api/api/select/mylasttimetouches.php";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            var json = JsonConvert.SerializeObject(lastTimeEntries);
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
                    
                    for(int i = 0; i < uObj.Count; i++)
                    {
                        var sbs = "";
                        var hex = "";
                        if (uObj[i]["Vorgang"].ToString() == "2")
                        {
                            sbs = "Kommen";
                            hex = "#84cc16";
                        }
                        else
                        {
                            sbs = "Gehen";
                            hex = "#ef4444";
                        }
                        Zeitbuchungens.Add(new Zeitbuchungen
                        {
                            Datum = uObj[i]["Datum"].ToString()+" - "+ uObj[i]["Uhrzeit"].ToString(),
                            Vorgang = uObj[i]["Vorgang"].ToString(),
                            Fall = sbs.ToString(),
                            HexCode = hex
                        });
                        
                        Console.WriteLine(uObj[i].ToString());
                    }
                    
                }                

            }
            
            BindingContext = this;
        }

        private async void MyCalender(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalenderOverview(user, null));
        }

        private async void RequestHolidays(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HolidayRequest(user));
        }

        private async void MyGoing(object sender, EventArgs e)
        {
            var b = DateTime.UtcNow;
            TimeZoneInfo timezone = TimeZoneInfo.Local;
            object value = b.ToLocalTime(); //Sets localtime of the system

            //String s = b.ToLocalTime().ToString("HH:mm"); //converts Time to 24 Hour format (6:53:28 => 18:53:28)
            String jahr = b.ToLocalTime().ToString("yyyy");
            String jahrkurz = b.ToLocalTime().ToString("yy");
            String monat = b.ToLocalTime().ToString("MM");
            String tag = b.ToLocalTime().ToString("dd");
            String monateinstellig = b.ToLocalTime().ToString("%M");
            String stunden = b.ToLocalTime().ToString("HH");
            String minuten = b.ToLocalTime().ToString("mm");
            String s = b.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            var extDateTime = s + ":00.000";
            var extDate = jahr + '-' + monateinstellig + '-' + tag + ' ' + "00:00:00.000";
            var vorgang = "3";
            var seter = "sc";
            var ImportDatum = s + ":00";
            var ctid = user.TimeTouchNr.ToString();
            var ctidlength = ctid.Length;
            var finalTid = "";
            if (ctidlength < 4)
            {
                for (int i = 0; i < (4 - ctidlength); i++)
                {
                    finalTid += "0";
                }
                finalTid += ctid;
            }
            else
            {
                finalTid = ctid;
            }
            var Buchung = "@t01ZB" + jahrkurz + monat + tag + stunden + minuten + "00" + vorgang + finalTid;
            var Uhrzeit = stunden + ":" + minuten;
            var Datum = tag + "." + monat + "." + jahrkurz;
            TimetouchEntry timetouchEntry = new TimetouchEntry
            {
                MID = user.Id,
                Personalnr = finalTid,
                Monat = monateinstellig,
                Jahr = jahr,
                Datum = Datum,
                Uhrzeit = Uhrzeit,
                Buchung = Buchung,
                ImportDatum = ImportDatum,
                Benutzer = seter,
                Vorgang = vorgang,
                ExtDate = extDate,
                ExtDateTime = extDateTime
            };
            var url = $"https://itsnando.com/api/api/insert/timetouchgoing.php";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            var json = JsonConvert.SerializeObject(timetouchEntry);
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

                    await DisplayAlert("Information", data.ToString(), "OK");
                    App.Current.MainPage = new NavigationPage(new Home(user));


                }
                else
                {
                    await DisplayAlert("Fehler", "Ein Fehler ist aufgetreten. Bitte versuchen Sie es erneut.", "OK");
                }

            }
        }

        private async void MyComing(object sender, EventArgs e)
        {
            var b = DateTime.UtcNow;
            TimeZoneInfo timezone = TimeZoneInfo.Local;
            object value = b.ToLocalTime(); //Sets localtime of the system

            //String s = b.ToLocalTime().ToString("HH:mm"); //converts Time to 24 Hour format (6:53:28 => 18:53:28)
            String jahr = b.ToLocalTime().ToString("yyyy");
            String jahrkurz= b.ToLocalTime().ToString("yy");
            String monat = b.ToLocalTime().ToString("MM");
            String tag = b.ToLocalTime().ToString("dd");
            String monateinstellig = b.ToLocalTime().ToString("%M");
            String stunden = b.ToLocalTime().ToString("HH");
            String minuten = b.ToLocalTime().ToString("mm");
            String s = b.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            var extDateTime = s + ":00.000";
            var extDate = jahr + '-' + monateinstellig + '-' + tag + ' ' + "00:00:00.000";
            var vorgang = "2";
            var seter = "sc";
            var ImportDatum= s + ":00";
            var ctid = user.TimeTouchNr.ToString();
            var ctidlength = ctid.Length;
            var finalTid = "";
            if (ctidlength < 4)
            {
                for(int i = 0; i < (4 - ctidlength); i++) {
                    finalTid += "0";
                }
                finalTid += ctid;
            }
            else
            {
                finalTid = ctid;
            }
            var Buchung = "@t01ZB" + jahrkurz + monat + tag + stunden + minuten + "00" + vorgang+ finalTid;
            var Uhrzeit = stunden + ":" + minuten;
            var Datum = tag + "." + monat + "." + jahrkurz;
            TimetouchEntry timetouchEntry = new TimetouchEntry
            {
                MID = user.Id,
                Personalnr=finalTid,
                Monat=monateinstellig,
                Jahr=jahr,
                Datum=Datum,
                Uhrzeit=Uhrzeit,
                Buchung=Buchung,
                ImportDatum=ImportDatum,
                Benutzer=seter,
                Vorgang=vorgang,
                ExtDate=extDate,
                ExtDateTime=extDateTime
            };
            var url = $"https://itsnando.com/api/api/insert/timetouchcoming.php";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            var json = JsonConvert.SerializeObject(timetouchEntry);
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
                    
                    await DisplayAlert("Information", data.ToString(), "OK");
                    App.Current.MainPage=new NavigationPage(new Home(user));


                }
                else
                {
                    await DisplayAlert("Fehler", "Ein Fehler ist aufgetreten. Bitte versuchen Sie es erneut.", "OK");
                }

            }
        }
    }
}