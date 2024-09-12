#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class SubmitJobCommand<T> :
    SubmitJob<T>
    where T : class
{
    public Guid JobId { get; set; }
    public T Job { get; set; } = null!;
    public RecurringJobSchedule? Schedule { get; set; }
}
