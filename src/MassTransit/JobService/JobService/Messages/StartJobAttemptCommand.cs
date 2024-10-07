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
    public Dictionary<string, object> Job { get; set; } = null!;
    public Guid JobTypeId { get; set; }
    public long? LastProgressValue { get; set; }
    public long? LastProgressLimit { get; set; }
    public Dictionary<string, object>? JobState { get; set; } = null!;
    public Dictionary<string, object>? JobProperties { get; set; }
}
