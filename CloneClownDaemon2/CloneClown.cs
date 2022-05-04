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
        public async Task Start()
        {
            User thisUser = await new UsersService().FindThisUser();
            if (thisUser == null)
            {
                await new UsersService().CreateThisUser();
                thisUser = await new UsersService().FindThisUser();
            }
            List<Configs> configs = thisUser.configs;
            Backup backup = new Backup();
            List<DateTime> dt = new List<DateTime>();
            string cronToUpdateUser = "*/5 * * * *";
            DateTime datetimeNextUpdateUser = new Scheduler().GetNextDateTime(cronToUpdateUser);
            foreach (Configs config in configs)
            {
                new MetadataManager(config).InitMetadata();
                dt.Add(new Scheduler().GetNextDateTime(config.schedule));
            }
            while (true)
            {
                for (int i = 0; i < configs.Count; i++)
                {
                    
                    if (DateTime.Compare(dt[i], DateTime.Now) < 0)
                    {
                        backup.Use(configs[i]);
                        dt[i] = new Scheduler().GetNextDateTime(configs[i].schedule);
                        thisUser.last_backup = DateTime.Now;
                        thisUser.configs[i].last_used = DateTime.Now;
                        //await new UsersService().Update(thisUser);
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
}
