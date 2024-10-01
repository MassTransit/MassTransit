#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class JobCompletedEvent :
    JobCompleted
{
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Job { get; set; } = null!;
    public Dictionary<string, object> Result { get; set; } = null!;
}


public class JobCompletedEvent<T> :
    JobCompleted<T>
    where T : class
{
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public T Job { get; set; } = null!;
    public Dictionary<string, object> Result { get; set; } = null!;
}
