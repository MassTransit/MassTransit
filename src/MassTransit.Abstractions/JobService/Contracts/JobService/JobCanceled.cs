namespace MassTransit.Contracts.JobService
{
    using System;


    /// <summary>
    /// Published when a job faults
    /// </summary>
    public interface JobCanceled
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The time the job was cancelled
        /// </summary>
        DateTime Timestamp { get; }
    }
}
