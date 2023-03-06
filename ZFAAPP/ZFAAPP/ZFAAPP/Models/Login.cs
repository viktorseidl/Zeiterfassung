using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZFAAPP.Models
{
    public class Login
    {
        public string PIN { get; set; }
        public string TID { get; set; }
    }
}
