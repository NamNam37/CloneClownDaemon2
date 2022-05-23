using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class DestF
    {
        public int id { get; set; }
        public int configID { get; set; }
        public string path { get; set; }
        public Type type { get; set; }
        public enum Type {
            local,
            ftp
        }


        public string login { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public int port { get; set; }

    }
}