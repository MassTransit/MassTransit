namespace MassTransit.Contracts.JobService
{
    using System;


    /// <summary>
    /// Published when a job is canceled
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

        /// <summary>
        /// THe reason, if specified, the job was canceled
        /// </summary>
        string? Reason { get; }
    }
}
