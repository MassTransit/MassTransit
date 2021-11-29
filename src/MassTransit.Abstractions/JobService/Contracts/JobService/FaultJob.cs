namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    [ConfigureConsumeTopology(false)]
    public interface FaultJob
    {
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
        /// The overall duration spent trying to process the job
        /// </summary>
        TimeSpan? Duration { get; }

        ExceptionInfo Exceptions { get; }

        /// <summary>
        /// The job, as an object dictionary
        /// </summary>
        IDictionary<string, object> Job { get; }

        /// <summary>
        /// The JobTypeId, to ensure the proper job type is started
        /// </summary>
        Guid JobTypeId { get; }
    }
}
