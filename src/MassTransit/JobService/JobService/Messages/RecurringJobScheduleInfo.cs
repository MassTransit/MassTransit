#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;
using Scheduling;


public class RecurringJobScheduleInfo :
    RecurringJobSchedule,
    IRecurringJobScheduleConfigurator,
    ISpecification
{
    public IEnumerable<ValidationResult> Validate()
    {
        var hasCronExpression = !string.IsNullOrWhiteSpace(CronExpression);

        if (!hasCronExpression && Start.HasValue == false)
            yield return this.Failure("CronExpression", "must be specified");

        if (Start.HasValue && End.HasValue && Start.Value > End.Value)
            yield return this.Failure("Start", "must be <= End");

        if (!hasCronExpression)
            yield break;

        ValidationResult? failure = null;
        try
        {
            _ = new CronExpression(CronExpression);
        }
        catch (FormatException exception)
        {
            failure = this.Failure("CronExpression", $"Is invalid: {exception.Message}");
        }

        if (failure != null)
            yield return failure;
    }

    public string? CronExpression { get; set; }
    public string? TimeZoneId { get; set; }
    public DateTimeOffset? Start { get; set; }
    public DateTimeOffset? End { get; set; }
}
