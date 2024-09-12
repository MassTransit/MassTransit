#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobStateResponse :
    JobState
{
    public Guid JobId { get; set; }
    public DateTime? Submitted { get; set; }
    public DateTime? Started { get; set; }
    public DateTime? Completed { get; set; }
    public DateTime? Faulted { get; set; }
    public string? Reason { get; set; }
    public int LastRetryAttempt { get; set; }
    public string CurrentState { get; set; } = null!;
}
