#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class SaveJobStateCommand :
    SaveJobState
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public Dictionary<string, object>? JobState { get; set; }
}
