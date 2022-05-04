using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class DestF
    {
        public int id { get; set; }
        public int configID { get; set; }
        public string path { get; set; }
        public virtual FTP  ftp { get; set; }
        public virtual Configs config { get; set; }
        public enum Type {
            local,
            ftp
            }

    }
}