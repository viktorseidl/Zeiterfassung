using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZFAAPP.Models;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Threading;
using System.Data;
using Picker = Xamarin.Forms.Picker;
using System.Windows.Input;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalenderOverview : ContentPage
    {
        
        public User user { get; set; }
        
        public string CallItem { get; set; }

        public ICommand CallDate { get; set; }

        public CalenderOverview(User benutzer, String mmonth)
        {
            InitializeComponent();
            user = benutzer;
            if (String.IsNullOrEmpty(mmonth)){
                var b = DateTime.UtcNow;
                String monat = b.ToLocalTime().ToString("MM");
                GetDataKal();
            }
            else
            {
                var b = DateTime.UtcNow;
                String monat = b.ToLocalTime().ToString("MM");
                NextDataKal(mmonth);
            }
            
            
            
            if (Preferences.ContainsKey("farb1"))
            {

            }
            else
            {   
                Preferences.Set("farb1", "#3498db");
                Preferences.Set("farb2", "#991b1b");
                Preferences.Set("farb4", "#d97706");
            }
        }
        
        public async void NextDataKal(string Monthy)
        {
            Refsh.IsRefreshing = true;
            var neu = DateTime.UtcNow;
            String jahr2 = neu.ToLocalTime().ToString("yyyy");
            DateTime dtneu = DateTime.ParseExact("01-" + Monthy + "-" + jahr2, "dd-MMM-yyyy", null);
            String monatneu = dtneu.ToLocalTime().ToString("MM");
            String jahrneu = dtneu.ToLocalTime().ToString("yyyy");
            var tagemonatneu = DateTime.DaysInMonth(Int32.Parse(jahrneu), Int32.Parse(monatneu));
            String jahrkurzneu = dtneu.ToLocalTime().ToString("yy");
            String monatmneu = dtneu.ToLocalTime().ToString("MMM");
            String tagneu = dtneu.ToLocalTime().ToString("dd");
            String monateinstelligneu = dtneu.ToLocalTime().ToString("%M");
            var WochentagStartMonatneu = dtneu.DayOfWeek.ToString("d"); //number 6 => Saturday
            String monatsnamneu = dtneu.ToLocalTime().ToString("MMMM");
            String stundenneu = dtneu.ToLocalTime().ToString("HH");
            String minutenneu = dtneu.ToLocalTime().ToString("mm");
            String sneu = dtneu.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            

            GetKalData getata = new GetKalData
            {
                MID = user.Id.ToString(),
                MTH = monateinstelligneu.ToString()
            };

            var url2 = $"https://itsnando.com/api/api/select/selectMyCal.php";
            var request2 = new HttpRequestMessage(HttpMethod.Post, url2);
            HttpClientHandler clientHandler2 = new HttpClientHandler();
            clientHandler2.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
            var client2 = new HttpClient(clientHandler2);
            var json2 = JsonConvert.SerializeObject(getata);
            var contentJson2 = new StringContent(json2, Encoding.UTF8, "application/json");
            var response2 = await client2.PostAsync(url2, contentJson2);
            if (response2.StatusCode == HttpStatusCode.OK)
            {
                var content = await response2.Content.ReadAsStringAsync();
                string cjson = content.ToString();
                var jObj = JObject.Parse(cjson);

                if (jObj.ContainsKey("data"))
                {
                    
                    var data = jObj["data"].ToString();
                    var uObj = JArray.Parse(data);
                    user.Buchungen = (string)uObj[0]["Belegung"];
                    user.RestUrlaub = (string)uObj[0]["RestUrlaub"];
                    user.SonderUrlaub = (string)uObj[0]["Sonderurlaub"];
                    user.Urlaubgesamt = (string)uObj[0]["Urlaubstage"];
                    user.Ausbezahlt = (string)uObj[0]["Ausbezahlt"];
                    Monatsname.Text = "- Urlaubstage gesamt: "+ user.Urlaubgesamt.ToString()+"\n"+ "- Resturlaub: " + user.RestUrlaub.ToString() + "\n"+ "- Sonderurlaub: " + user.SonderUrlaub.ToString() + "\n";
                    var strsplt = user.Buchungen;
                    char[] characters = strsplt.ToCharArray();
                    Label[] labelsArray = { T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42 };
                    var counterDays = 0;
                    for (int i = 0; i < 42; i++)
                    {
                        var neuI = i + 1;
                        if (neuI < Int32.Parse(WochentagStartMonatneu))
                        {
                            labelsArray[i].Text = "";
                            labelsArray[i].BackgroundColor = Color.Transparent;
                        }
                        else if (neuI >= tagemonatneu + Int32.Parse(WochentagStartMonatneu))
                        {
                            labelsArray[i].Text = "";
                            labelsArray[i].BackgroundColor = Color.Transparent;
                        }
                        else
                        {
                            if (characters[counterDays].ToString() == "1")
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex(Preferences.Get("farb1", ""));
                            }
                            else if (characters[counterDays].ToString() == "2")
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex(Preferences.Get("farb2", ""));
                            }
                            else if (characters[counterDays].ToString() == "4")
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex(Preferences.Get("farb4", ""));
                            }
                            else
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex("#475569");
                            }
                            counterDays++;

                            labelsArray[i].Text = counterDays.ToString();
                        }
                    }

                }
                else
                {
                    //Error on Database Connection or wrong query
                    var data = jObj["message"].ToString();
                    await DisplayAlert("Fehlermeldung", data, "OK");
                }
                Refsh.IsRefreshing = false;
            }
        }
        public async void GetDataKal()
        {
            Refsh.IsRefreshing = true;
            var b = DateTime.UtcNow;
                String jahr = b.ToLocalTime().ToString("yyyy");
                String jahrkurz = b.ToLocalTime().ToString("yy");
                String monat = b.ToLocalTime().ToString("MM");
                String monatm = b.ToLocalTime().ToString("MMM");
                String tag = b.ToLocalTime().ToString("dd");
                var tagemonat = DateTime.DaysInMonth(Int32.Parse(jahr), Int32.Parse(monat));
                String monateinstellig = b.ToLocalTime().ToString("%M");
                DateTime dt = DateTime.ParseExact("01-" + monatm + "-" + jahr, "dd-MMM-yyyy", null);
                var WochentagStartMonat = dt.DayOfWeek.ToString("d"); //number 6 => Saturday
                String monatsnam = b.ToLocalTime().ToString("MMMM");
                String stunden = b.ToLocalTime().ToString("HH");
                String minuten = b.ToLocalTime().ToString("mm");
                String s = b.ToLocalTime().ToString("yyyy-MM-dd HH:mm");            
            GetKalData getKaldata = new GetKalData
                {
                    MID = user.Id.ToString(),
                    MTH = monateinstellig.ToString()
                };            
            var url = $"https://itsnando.com/api/api/select/selectMyCal.php";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            var json = JsonConvert.SerializeObject(getKaldata);
            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, contentJson);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                string cjson = content.ToString();
                var jObj = JObject.Parse(cjson);

                if (jObj.ContainsKey("data"))
                {
                    //User can be logged in
                    var data = jObj["data"].ToString();
                    var uObj = JArray.Parse(data);
                    user.Buchungen = (string)uObj[0]["Belegung"];
                    user.RestUrlaub = (string)uObj[0]["RestUrlaub"];
                    user.SonderUrlaub = (string)uObj[0]["Sonderurlaub"];
                    user.Urlaubgesamt = (string)uObj[0]["Urlaubstage"];
                    user.Ausbezahlt = (string)uObj[0]["Ausbezahlt"];
                    Monatsname.Text = "- Urlaubstage gesamt: " + user.Urlaubgesamt.ToString() + "\n" + "- Resturlaub: " + user.RestUrlaub.ToString() + "\n" + "- Sonderurlaub: " + user.SonderUrlaub.ToString() + "\n";
                    var strsplt = user.Buchungen;
                    char[] characters = strsplt.ToCharArray();
                    Label[] labelsArray = { T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42 };
                    var counterDays = 0;
                    for (int i = 0; i < 42; i++)
                    {
                        var neuI = i + 1;
                        if (neuI < Int32.Parse(WochentagStartMonat))
                        {
                            labelsArray[i].Text = "";
                            //kalenderViewint.Children.Add(new Label() { Text = "Hallo", BackgroundColor = Color.Beige, Grid.Column ="" });
                        }
                        else if (neuI >= tagemonat+ Int32.Parse(WochentagStartMonat))
                        {
                             labelsArray[i].Text = "";
                            //kalenderViewint.Children.Add(new Label() { Text = "Hallo", BackgroundColor = Color.Beige, Grid.Column ="" });
                        }
                        else
                        {
                            if (characters[counterDays].ToString() == "1")
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex(Preferences.Get("farb1", ""));
                                var blue = ColorConverters.FromHex("#3498db");
                            }
                            else if (characters[counterDays].ToString() == "2")
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex(Preferences.Get("farb2", ""));
                            }
                            else if (characters[counterDays].ToString() == "4")
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex(Preferences.Get("farb4", ""));
                            }
                            else
                            {
                                labelsArray[i].BackgroundColor = Color.FromHex("#475569");
                            }
                            counterDays++;
                            
                            labelsArray[i].Text = counterDays.ToString();
                        }
                    }
                }
                else
                {
                    //Error on Database Connection or wrong query
                    var data = jObj["message"].ToString();
                    await DisplayAlert("Fehlermeldung", data, "OK");
                }
            }
            Refsh.IsRefreshing = false;
        }

        private void BtnBack(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new Home(user));
        }


        private void Jan(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user,"Jan"));
        }

        private void Feb(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Feb"));
        }

        private void Mar(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Mär"));
        }

        private void Apr(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Apr"));
        }

        private void May(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Mai"));
        }

        private void Jun(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Jun"));
        }

        private void Jul(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Jul"));
        }

        private void Aug(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Aug"));
        }

        private void Sep(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Sep"));
        }

        private void Okt(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Okt"));
        }

        private void Nov(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Nov"));
        }

        private void Dez(object sender, EventArgs e)
        {
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new CalenderOverview(user, "Dez"));
        }
    }
    
}