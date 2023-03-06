using System;
using System.Collections.Generic;
using System.Text;

namespace ZFAAPP.Models
{
    public class SupportTicket
    {
        public int Mid { get; set; }
        public string Mvname { get; set; }
        public string Mnname { get; set; }

        public string Mail { get; set; }

        public string SReason { get; set; }

        public string SDescription { get; set; }
    }
}
