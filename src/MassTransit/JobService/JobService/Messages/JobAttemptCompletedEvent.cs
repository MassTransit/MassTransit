#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobAttemptCompletedEvent :
    JobAttemptCompleted
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public int RetryAttempt { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
}
