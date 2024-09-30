#nullable enable
namespace MassTransit;

using System;


/// <summary>
/// Configure the optional settings of a recurring job
/// </summary>
public interface IRecurringJobScheduleConfigurator
{
    /// <summary>
    /// A valid cron expression specifying the job schedule
    /// </summary>
    public string CronExpression { set; }

    /// <summary>
    /// If specified, the start date for the job. Otherwise, the current date/time will be used.
    /// </summary>
    public DateTimeOffset? Start { set; }

    /// <summary>
    /// If specified, the end date for the job after which it will be removed from the job scheduler
    /// </summary>
    public DateTimeOffset? End { set; }

    /// <summary>
    /// If specified, the time zone in which the cron expression should be evaluated, otherwise UTC is used.
    /// </summary>
    string? TimeZoneId { set; }
}


