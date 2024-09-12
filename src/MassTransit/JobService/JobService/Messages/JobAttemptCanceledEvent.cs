#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobAttemptCanceledEvent :
    JobAttemptCanceled
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public int RetryAttempt { get; set; }
    public DateTime Timestamp { get; set; }
}
