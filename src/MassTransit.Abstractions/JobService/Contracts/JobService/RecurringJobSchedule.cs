namespace MassTransit.Contracts.JobService;

using System;


public interface RecurringJobSchedule
{
    /// <summary>
    /// A valid cron expression specifying the job schedule
    /// </summary>
    string? CronExpression { get; }

    /// <summary>
    /// If specified, the time zone in which the cron expression should be evaluated, otherwise UTC is used.
    /// </summary>
    string? TimeZoneId { get; }

    /// <summary>
    /// If specified, the start date for the job. Otherwise, the current date/time will be used.
    /// </summary>
    DateTimeOffset? Start { get; }

    /// <summary>
    /// If specified, the end date for the job after which it will be removed from the job scheduler
    /// </summary>
    DateTimeOffset? End { get; }
}
