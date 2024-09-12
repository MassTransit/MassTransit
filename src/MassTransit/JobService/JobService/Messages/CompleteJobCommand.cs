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
    public IDictionary<string, object> Job { get; set; } = null!;
    public IDictionary<string, object> Result { get; set; } = null!;
    public Guid JobTypeId { get; set; }
}
