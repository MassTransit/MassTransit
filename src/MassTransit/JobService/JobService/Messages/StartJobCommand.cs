#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class StartJobCommand :
    StartJob
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public int RetryAttempt { get; set; }
    public Dictionary<string, object> Job { get; set; } = null!;
    public Guid JobTypeId { get; set; }
    public long? LastProgressValue { get; set; }
    public long? LastProgressLimit { get; set; }
    public Dictionary<string, object>? JobState { get; set; }
    public Dictionary<string, object>? JobProperties { get; set; }
}
