namespace MassTransit.Scheduling
{
    using System;


    public interface RecurringSchedule
    {
        /// <summary>
        /// The timezone of the schedule
        /// </summary>
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
        /// A unique name that identifies this schedule.
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

        /// <summary>
        /// Schedule description
        /// </summary>
        string Description { get; }

        MissedEventPolicy MisfirePolicy { get; }
    }
}
