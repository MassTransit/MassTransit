#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobAttemptStatusResponse :
    JobAttemptStatus
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public DateTime Timestamp { get; set; }
    public JobStatus Status { get; set; }
}
