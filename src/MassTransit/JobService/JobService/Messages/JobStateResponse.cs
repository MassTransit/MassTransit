#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class JobStateResponse :
    JobState
{
    public Guid JobId { get; set; }
    public DateTime? Submitted { get; set; }
    public DateTime? Started { get; set; }
    public DateTime? Completed { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? Faulted { get; set; }
    public string? Reason { get; set; }
    public int LastRetryAttempt { get; set; }
    public string CurrentState { get; set; } = null!;
    public long? ProgressValue { get; set; }
    public long? ProgressLimit { get; set; }
    public Dictionary<string, object>? JobState { get; set; }
    public DateTime? NextStartDate { get; set; }
    public bool IsRecurring { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}


public class JobStateResponse<T> :
    JobState<T>
    where T : class
{
    readonly JobState _jobState;
    readonly T? _jobStateOfT;

    public JobStateResponse(JobState jobState, T? jobStateOfT = null)
    {
        _jobState = jobState;
        _jobStateOfT = jobStateOfT;
    }

    public Guid JobId => _jobState.JobId;
    public DateTime? Submitted => _jobState.Submitted;
    public DateTime? Started => _jobState.Started;
    public DateTime? Completed => _jobState.Completed;
    public TimeSpan? Duration => _jobState.Duration;
    public DateTime? Faulted => _jobState.Faulted;
    public string? Reason => _jobState.Reason;
    public int LastRetryAttempt => _jobState.LastRetryAttempt;
    public string CurrentState => _jobState.CurrentState;
    public long? ProgressValue => _jobState.ProgressValue;
    public long? ProgressLimit => _jobState.ProgressLimit;
    public Dictionary<string, object>? JobState => _jobState.JobState;
    public DateTime? NextStartDate => _jobState.NextStartDate;
    public bool IsRecurring => _jobState.IsRecurring;
    public DateTime? StartDate => _jobState.StartDate;
    public DateTime? EndDate => _jobState.EndDate;
    T? JobState<T>.JobState => _jobStateOfT;
}
