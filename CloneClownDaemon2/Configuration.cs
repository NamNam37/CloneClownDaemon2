using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class Configuration
    {
        public string name { get; set; }
        public int foldersMax { get; set; }
        public int rollbackMax { get; set; }
        public List<string> sources { get; set; }
        public List<string> dests { get; set; }
        public string cron { get; set; }
        public bool ZIP { get; set; }
        public Type type { get; set; }
        public enum Type
        {
            Full,
            Differencial,
            Incremental
        }
        public void SetDemoConfig()
        {
            name = "demo";
            foldersMax = 3;
            rollbackMax = 4;
            ZIP = true;
            type = Type.Full;
            string source = @"C:/Users/david/Music/source";
            string source2 = @"C:/Users/david/Music/source2";
            string dest = @"C:/Users/david/Music/dest";
            string dest2 = @"C:/Users/david/Music/dest2";
            sources = new List<string>() { source, source2 };
            dests = new List<string>() { dest, dest2 };
        }
    }
}
