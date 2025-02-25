#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class SetConcurrentJobLimitCommand :
    SetConcurrentJobLimit
{
    public Guid JobTypeId { get; set; }
    public Uri InstanceAddress { get; set; } = null!;
    public int ConcurrentJobLimit { get; set; }
    public ConcurrentLimitKind Kind { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? JobTypeName { get; set; }
    public Dictionary<string, object>? JobTypeProperties { get; set; }
    public Dictionary<string, object>? InstanceProperties { get; set; }
    public int? GlobalConcurrentJobLimit { get; set; }
}
