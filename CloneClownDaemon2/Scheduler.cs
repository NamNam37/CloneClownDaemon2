using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class Scheduler
    {
        public DateTime GetNextDateTime(string cron)
        {
            CrontabSchedule schedule = CrontabSchedule.Parse(cron);
            return schedule.GetNextOccurrence(DateTime.Now);
        }
    }
}
