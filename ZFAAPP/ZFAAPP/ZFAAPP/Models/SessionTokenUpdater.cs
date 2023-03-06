using System;
using System.Collections.Generic;
using System.Text;

namespace ZFAAPP.Models
{
    public class SessionTokenUpdater
    {
        //User ID
        public int id { get; set; }
        //API timetouchHash
        public string API { get; set; }
        public int qType = 4;
        //Session Token
        public string NRQ { get; set; }

    }
}
