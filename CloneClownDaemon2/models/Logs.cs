using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class Logs
    {
        public int id { get; set; }
        public int userID { get; set; }
        public int configID { get; set; }
        public bool status { get; set; }
        public string details { get; set; }
        public DateTime date { get; set; }
        public bool alreadySent { get; set; }

        public virtual User user { get; set; }
        public virtual Configs config { get; set; }

    }
}