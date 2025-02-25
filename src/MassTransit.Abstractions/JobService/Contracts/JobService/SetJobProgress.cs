namespace MassTransit.Contracts.JobService;

using System;


public interface SetJobProgress
{
    Guid JobId { get; }

    Guid AttemptId { get; }

    long SequenceNumber { get; }

    /// <summary>
    /// The current job progress value
    /// </summary>
    long Value { get; }

    /// <summary>
    /// The maximum value of job progress (optional)
    /// </summary>
    long? Limit { get; }
}
