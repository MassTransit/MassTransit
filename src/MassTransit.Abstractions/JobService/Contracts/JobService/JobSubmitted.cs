namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    public interface JobSubmitted
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        Guid JobTypeId { get; }

        /// <summary>
        /// The time the job was submitted
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Timeout when running job
        /// </summary>
        TimeSpan JobTimeout { get; }

        /// <summary>
        /// The job, as an object dictionary
        /// </summary>
        IDictionary<string, object> Job { get; }
    }
}
