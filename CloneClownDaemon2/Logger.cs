using CloneClownAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class Logger
    {
        public async Task SuccesfulLog(User user, Configs config)
        {
            await new LogsService().Create(new Logs(user.id, config.id, true, 0, DateTime.Now, false));
        }
        public async Task FailedLog(User user, Configs config, int errorCode)
        {
            if (config == null)
            {
                await new LogsService().Create(new Logs(user.id, null, false, errorCode, DateTime.Now, false));
            }
            else
            {
                await new LogsService().Create(new Logs(user.id, config.id, false, errorCode, DateTime.Now, false));
            }
        }
    }
}
