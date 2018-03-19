using System;

namespace PirateX.Schedule
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CronScheduleAttribute:Attribute
    {
        public string CronSchedule { get; private set; }

        public CronScheduleAttribute( string cronSchedule)
        {
            if(string.IsNullOrEmpty(cronSchedule))
                throw new ArgumentNullException(nameof(cronSchedule));
            
            this.CronSchedule = cronSchedule;
        }
    }
}
