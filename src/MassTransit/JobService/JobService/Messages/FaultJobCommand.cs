#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class FaultJobCommand :
    FaultJob
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public int RetryAttempt { get; set; }
    public TimeSpan? Duration { get; set; }
    public ExceptionInfo Exceptions { get; set; } = null!;
    public Dictionary<string, object> Job { get; set; } = null!;
    public Guid JobTypeId { get; set; }
}
