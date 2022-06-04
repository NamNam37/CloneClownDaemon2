using CloneClownAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class CloneClown
    {
        DateTime datetimeNextUpdateUser { get; set; }
        List<DateTime> dt { get; set; }
        List<Configs> configs { get; set; }
        User thisUser { get; set; }
        string cronToUpdateUser = "* * * * *";
        public async Task Init()
        {
            thisUser = await new UsersService().FindThisUser();
            if (thisUser == null)
            {
                await new UsersService().CreateThisUser();
                thisUser = await new UsersService().FindThisUser();
            }
            configs = thisUser.configs;
            dt = new List<DateTime>();
            foreach (Configs config in configs)
            {
                new MetadataManager(config).InitMetadata();
                dt.Add(new Scheduler().GetNextDateTime(config.schedule));
            }
            
            datetimeNextUpdateUser = new Scheduler().GetNextDateTime(cronToUpdateUser);
        }
        public async Task Start()
        {
            

            while (true)
            {
                if (thisUser.verified)
                {
                    for (int i = 0; i < configs.Count; i++)
                    {

                        if (DateTime.Compare(dt[i], DateTime.Now) < 0)
                        {
                            Backup backup = new Backup(configs[i]);
                            backup.Use();
                            dt[i] = new Scheduler().GetNextDateTime(configs[i].schedule);
                            thisUser.last_backup = DateTime.Now;
                            thisUser.configs[i].last_used = DateTime.Now;
                        }

                    }
                }
                if (DateTime.Compare(datetimeNextUpdateUser, DateTime.Now) < 0)
                {
                    thisUser = await new UsersService().FindThisUser();
                    configs = thisUser.configs;

                    datetimeNextUpdateUser = new Scheduler().GetNextDateTime(cronToUpdateUser);
                }
            }
            
            
        }
    }
}
