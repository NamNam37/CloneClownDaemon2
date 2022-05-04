using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class FTP
    {
        public int id { get; set; }
        public int destID { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public virtual DestF destF { get; set; }
    }
}