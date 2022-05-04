using System;
using System.Collections.Generic;

namespace CloneClownAPI.Models
{
    public class Configs
    {
        public int id { get; set; }
        public string configName { get; set; }
        public DateTime last_used { get; set; }
        public string schedule { get; set; }
        public enum Type
        {
            full,
            differencial,
            incremental
        }
        public Type type { get; set; }
        public int backupCount { get; set; }
        public int packageCount { get; set; }
        public bool isZIP { get; set; }
        public virtual List<SourceF> sources { get; set; }
        public virtual List<DestF> dests { get; set; }

        public virtual List<User> users { get; set; }
        public virtual List<Logs> logs { get; set; }
        public Configs(string configName, DateTime last_used, string schedule, Type type, int backupCount, int packageCount, bool isZIP, List<User> users, List<Logs> logs)
        {
            this.configName = configName;
            this.last_used = last_used;
            this.schedule = schedule;
            this.type = type;
            this.backupCount = backupCount;
            this.packageCount = packageCount;
            this.isZIP = isZIP;
            this.users = users;
            this.logs = logs;
        }
        public Configs()
        {

        }
    }
}