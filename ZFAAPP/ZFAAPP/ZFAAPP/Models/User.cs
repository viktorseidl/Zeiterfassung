using System;
using System.Collections.Generic;
using System.Text;

namespace ZFAAPP.Models
{
    public class User
    {
        public int Id { get; set; }
        public string V_name { get; set; }
        public string N_name { get; set; }

        public int TimeTouchNr { get; set; }
        private string PIN { get; set; }

        private string TID { get; set; }

        public string Buchungen { get; set; }

        public string RestUrlaub { get; set; }

        public string SonderUrlaub { get; set; }

        public string Urlaubgesamt { get; set; }

        public string Ausbezahlt { get; set; }

        public bool LogState = false;

        public void SetKeysOnUser(string Pin, string Tid)
        {
            PIN = Pin.Trim();
            TID = Tid.Trim();
        }
        public string GetHashPass()
        {
            return PIN;
        }
        public string GetHashTid()
        {
            return TID;
        }
    }
}
