#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class JobFaultedEvent :
    JobFaulted
{
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan? Duration { get; set; }
    public Dictionary<string, object> Job { get; set; } = null!;
    public ExceptionInfo Exceptions { get; set; } = null!;
}
