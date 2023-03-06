using System;
using System.Collections.Generic;
using System.Text;

namespace ZFAAPP.Models
{
    public class TimetouchEntry
    {
        public int MID { get; set; }
        public string Personalnr { get; set; }
        public string Monat { get; set; }

        public string Jahr { get; set; }

        public string Datum { get; set; }
        public string Uhrzeit { get; set; }

        public string Buchung { get; set; }

        public string ImportDatum { get; set; }

        public string Benutzer { get; set; } 

        public string Vorgang { get; set; }
        public string ExtDate { get; set; }

        public string ExtDateTime { get; set; }
    }
}
