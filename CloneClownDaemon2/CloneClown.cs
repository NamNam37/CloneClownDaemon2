using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class CloneClown
    {
        public void Start()
        {
            Configuration config = new Configuration();
            config.SetDemoConfig();
            new MetadataManager(config).InitMetadata();

            Backup backup = new Backup();
            while (true)
            {
                ConsoleKey input = Console.ReadKey(true).Key;
                
                if (input == ConsoleKey.B)
                {
                    Console.WriteLine("Backup In Progress");
                    backup.Use(config);
                    Console.WriteLine("Done");
                }
                
            }
        }
    }
}
