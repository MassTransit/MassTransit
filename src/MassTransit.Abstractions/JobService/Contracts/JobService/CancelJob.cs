namespace MassTransit.Contracts.JobService
{
    using System;


    public interface CancelJob
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The reason for cancelling the job
        /// </summary>
        string? Reason { get; }
    }
}
