using System;
using System.Collections.Generic;
using System.Text;

namespace ZFAAPP.Models
{
    public class HolidayAntrag
    {
        public int MID { get; set; }
        public string Datum { get; set; }
        public string BisDatum { get; set; }

        public string Mvname { get; set; }

        public string Mnname { get; set; }

        public string Antragtyp { get; set; }

        public string AnzahlTage { get; set; }

        public string Bemerkung { get; set; }
    }
}
