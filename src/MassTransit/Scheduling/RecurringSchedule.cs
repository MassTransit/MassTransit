using System;

namespace MassTransit.Scheduling
{
 
    public abstract class RecurringSchedule : IRecurringSchedule
    {
        protected RecurringSchedule()
        {
            ScheduleId = this.GetType().FullName;
            ScheduleGroup = this.GetType().Assembly.FullName.Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries)[0];
        }

        public  string TimeZoneId { get;protected set; }
        public  DateTimeOffset StartTime { get; protected set; }
        public  DateTimeOffset? EndTime { get; protected set; }
        public string ScheduleId { get; private set; }
        public string ScheduleGroup { get; private set; }
        public  string CronExpression { get; protected set; }
        public  MisfireInstruction MisfirePolicy { get; protected set; }
    }
}