using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class Logs
    {
        public int id { get; set; }
        public int userID { get; set; }
        public int? configID { get; set; }
        public bool status { get; set; }
        public int details { get; set; }
        public DateTime date { get; set; }
        public bool alreadySent { get; set; }

        public virtual User user { get; set; }
        public virtual Configs? config { get; set; }

        public Logs() { }

        public Logs(int userID, int? configID, bool status, int details, DateTime date, bool alreadySent)
        {
            this.userID = userID;
            this.configID = configID;
            this.status = status;
            this.details = details;
            this.date = date;
            this.alreadySent = alreadySent;
        }
    }
}