#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class RecurringJobScheduleInfo
    :RecurringJobSchedule
{
    public string? CronExpression { get; set; }
    public string? TimeZoneId { get; set; }
    public DateTimeOffset? Start { get; set; }
    public DateTimeOffset? End { get; set; }
}
