#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobAttemptFaultedEvent :
    JobAttemptFaulted
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public int RetryAttempt { get; set; }
    public TimeSpan? RetryDelay { get; set; }
    public DateTime Timestamp { get; set; }
    public ExceptionInfo Exceptions { get; set; } = null!;
}
