namespace MassTransit.Scheduling
{
    using System;
    using System.Reflection;


    public abstract class DefaultRecurringSchedule :
        RecurringSchedule
    {
        protected DefaultRecurringSchedule()
        {
            ScheduleId = TypeCache.GetShortName(GetType());
            ScheduleGroup = GetType().GetTypeInfo().Assembly.FullName.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];

            TimeZoneId = TimeZoneInfo.Local.Id;
            StartTime = DateTime.Now;
        }

        public MissedEventPolicy MisfirePolicy { get; protected set; }
        public string TimeZoneId { get; protected set; }
        public DateTimeOffset StartTime { get; protected set; }
        public DateTimeOffset? EndTime { get; protected set; }
        public string ScheduleId { get; private set; }
        public string ScheduleGroup { get; private set; }
        public string CronExpression { get; protected set; }
        public string Description { get; protected set; }
    }
}
