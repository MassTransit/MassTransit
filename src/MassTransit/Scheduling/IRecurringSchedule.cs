using System;

namespace MassTransit.Scheduling
{
    public interface IRecurringSchedule
    {
        string TimeZoneId { get; }

        /// <summary>
        /// The time the recurring schedule is enabled
        /// </summary>
        DateTimeOffset StartTime { get; }

        /// <summary>
        /// The time the recurring schedule is disabled
        /// If null then the job is repeated forever
        /// </summary>
        DateTimeOffset? EndTime { get; }

        /// <summary>
        /// A unique name that idenifies this schedule.
        /// </summary>
        string ScheduleId { get; }

        /// <summary>
        /// A
        /// </summary>
        string ScheduleGroup { get; }

        /// <summary>
        /// The Cron Schedule Expression in Cron Syntax
        /// </summary>
        string CronExpression { get; }

        MisfireInstruction MisfirePolicy { get; }
    }
}