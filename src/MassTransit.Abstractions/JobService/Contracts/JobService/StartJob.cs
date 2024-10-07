namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    [ConfigureConsumeTopology(false)]
    public interface StartJob
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
        /// The job, as an object dictionary
        /// </summary>
        Dictionary<string, object> Job { get; }

        /// <summary>
        /// The JobTypeId, to ensure the proper job type is started
        /// </summary>
        Guid JobTypeId { get; }

        /// <summary>
        /// The last reported progress value from a previous job execution
        /// </summary>
        long? LastProgressValue { get; }

        /// <summary>
        /// The last reported progress limit
        /// </summary>
        long? LastProgressLimit { get; }

        /// <summary>
        /// The job state, as an object dictionary
        /// </summary>
        Dictionary<string, object>? JobState { get; }

        /// <summary>
        /// The job properties, supplied when the job was submitted
        /// </summary>
        Dictionary<string, object>? JobProperties { get; }
    }
}
