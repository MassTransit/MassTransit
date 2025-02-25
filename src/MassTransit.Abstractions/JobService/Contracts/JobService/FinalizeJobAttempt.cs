namespace MassTransit.Contracts.JobService;

using System;


public interface FinalizeJobAttempt
{
    /// <summary>
    /// The job identifier
    /// </summary>
    Guid JobId { get; }

    /// <summary>
    /// Identifies this attempt to run the job
    /// </summary>
    Guid AttemptId { get; }
}
