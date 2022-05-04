using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class SourceF
    {
        public int id { get; set; }
        public int configID { get; set; }
        public string path { get; set; }
        public virtual Configs config { get; set; }
    }
}