namespace MassTransit.Contracts.JobService
{
    using System;


    /// <summary>
    /// Event published when a node starts processing a job
    /// </summary>
    public interface JobStarted
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// Identifies this attempt to run the job
        /// </summary>
        Guid AttemptId { get; }

        /// <summary>
        /// Zero if the job is being started for the first time, otherwise, the number of previous failures
        /// </summary>
        int RetryAttempt { get; }

        /// <summary>
        /// The time the job was started
        /// </summary>
        DateTime Timestamp { get; }
    }


    /// <summary>
    /// Event published when a node starts processing a job (separately from <see cref="JobStarted"/>)
    /// </summary>
    public interface JobStarted<T>
        where T : class
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// Identifies this attempt to run the job
        /// </summary>
        Guid AttemptId { get; }

        /// <summary>
        /// Zero if the job is being started for the first time, otherwise, the number of previous failures
        /// </summary>
        int RetryAttempt { get; }

        /// <summary>
        /// The time the job was started
        /// </summary>
        DateTime Timestamp { get; }
    }
}
