namespace MassTransit.Contracts.JobService;

using System;
using System.Collections.Generic;


public interface SaveJobState
{
    Guid JobId { get; }

    Guid AttemptId { get; }

    /// <summary>
    /// The state of the job, as a dictionary, or null to clear the state
    /// </summary>
    Dictionary<string, object>? JobState { get; }
}
