namespace MassTransit.Contracts.JobService
{
    using System;


    public interface GetJobState
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }
    }
}
