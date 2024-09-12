#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class StartJobAttemptCommand :
    StartJobAttempt
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public int RetryAttempt { get; set; }
    public Uri ServiceAddress { get; set; } = null!;
    public Uri InstanceAddress { get; set; } = null!;
    public IDictionary<string, object> Job { get; set; } = null!;
    public Guid JobTypeId { get; set; }
}
