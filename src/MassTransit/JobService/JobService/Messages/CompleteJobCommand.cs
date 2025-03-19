#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class CompleteJobCommand :
    CompleteJob
{
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Job { get; set; } = null!;
    public Guid JobTypeId { get; set; }
    public Dictionary<string, object>? JobProperties { get; set; }
    public Dictionary<string, object>? InstanceProperties { get; set; }
    public Dictionary<string, object>? JobTypeProperties { get; set; }
}
