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
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace ZFAAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassChange : ContentPage
    {
        public User user { get; set; }
        public PassChange(User benutzer)
        {
            InitializeComponent();
            user = benutzer;
        }

        private void btnBackHome(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        public static bool useRegex(String input)
        {
            string rege = "";
            int counter=0;
            for (int CaseTest = 1; CaseTest < 5; CaseTest++)
            {
                if (CaseTest == 1)
                {
                    //lowerCase
                    rege = "(.*[a-z].*){1}$";
                    Regex regex = new Regex(rege);
                    Console.WriteLine(regex.IsMatch(input).ToString()+" lowercase");
                    if (regex.IsMatch(input) == true)
                    {
                        counter++;
                    }
                }
                else if (CaseTest == 2)
                {
                    //upperCase
                    rege = "(.*[A-Z].*){1}$";
                    Regex regex = new Regex(rege);
                    Console.WriteLine(regex.IsMatch(input).ToString() + " uppercase");
                    if (regex.IsMatch(input) == true)
                    {
                        counter++;
                    }
                }
                else if (CaseTest == 3)
                {
                    //Number
                    rege = @"^(.*[\d].*){1}$";
                    Regex regex = new Regex(rege);
                    Console.WriteLine(regex.IsMatch(input).ToString() + " Number");
                    if (regex.IsMatch(input) == true)
                    {
                        counter++;
                    }
                }
                else
                {
                    //Special Characters
                    rege = @"(.*[!?@_.:%$#+*].*){1}$";
                    Regex regex = new Regex(rege);
                    Console.WriteLine(regex.IsMatch(input).ToString() + " S chars");
                    if (regex.IsMatch(input) == true)
                    {
                        counter++;
                    }
                }                
            }
            if(counter == 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async void btnChangePass(object sender, EventArgs e)
        {
            var OldPa = OldPass.Text;
            var NewPa = Newpass.Text;
            var NewPaA = NewpassAgain.Text;
            if (String.IsNullOrEmpty(OldPa) == true || String.IsNullOrEmpty(NewPa) == true || String.IsNullOrEmpty(NewPaA) == true )
            {
                await DisplayAlert("Fehler", "Bitte füllen Sie alle Felder aus!", "OK");
            }
            else if(NewPa != NewPaA)
            {
                await DisplayAlert("Fehler", "Neues Passwort stimt nicht mit wiederholten Passwoert überein!", "OK");
            }
            else if(NewPa == OldPa)
            {
                await DisplayAlert("Fehler", "Das Passwort neue Passwort, darf nicht das selbe wie das bisherige Passwort sein!", "OK");
            }
            else if (NewPa.Length < 8)
            {
                await DisplayAlert("Fehler", "Das Passwort muss mindestens 8 Zeichen lang sein!", "OK");
            }
            else if(useRegex(NewPa)==false)  
            {
                await DisplayAlert("Fehler", "Das Passwort muss mindestens 1 Sonderzeichen enthalten (!?@_.:%$#+*), eine Zahl (0-9), Groß und Kleinbuchstaben und mindestens 8 Zeichen lang sein.", "OK");
            }
            else
            {
                
                Console.WriteLine(WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes("Test"))));
                NewPass newPass = new NewPass
                {
                    MID = WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(user.TimeTouchNr.ToString()))),
                    PP = WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(OldPass.Text))),
                    PPN = WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(Newpass.Text))),
                    EM = WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(Mail.Text)))
                };
                var url = $"https://itsnando.com/api/api/update/twofactorupdatePass.php";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, sslPolicyErrors) => { return true; };
                var client = new HttpClient(clientHandler);
                var json = JsonConvert.SerializeObject(newPass);
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
                        user = null;
                        User benutzer = new User();
                        benutzer.LogState = false;
                        await Navigation.PopAsync();
                        App.Current.MainPage = new NavigationPage(new MainPage(benutzer));
                    }
                    else
                    {
                        await DisplayAlert("Fehler", "Ein Fehler ist aufgetreten. Bitte versuchen Sie es erneut. Sollte der Fehler weiterhin bestehen, dann kontaktieren Sie den Support.", "OK");
                    }

                }
            }

        }
    }
}