using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string IP { get; set; }
        public bool online { get; set; }
        public DateTime last_backup { get; set; }

        public  List<Configs> configs { get; set; }
        public List<Logs> logs { get; set; }
        public User(string username, string IP, bool online, DateTime last_backup, List<Configs> configs, List<Logs> logs)
        {
            this.username = username;
            this.IP = IP;
            this.online = online;
            this.last_backup = last_backup;
            this.configs = configs;
            this.logs = logs;
        }
        public User()
        {

        }
        
    }
}