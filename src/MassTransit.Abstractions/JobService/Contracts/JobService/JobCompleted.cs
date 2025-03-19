namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a job completes
    /// </summary>
    public interface JobCompleted
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        DateTime Timestamp { get; }

        TimeSpan Duration { get; }

        /// <summary>
        /// The arguments used to start the job
        /// </summary>
        Dictionary<string, object> Job { get; }

        /// <summary>
        /// Properties specified for this job
        /// </summary>
        Dictionary<string, object>? JobProperties { get; }

        /// <summary>
        /// Properties of the instance that completed the job
        /// </summary>
        Dictionary<string, object>? InstanceProperties { get; }

        /// <summary>
        /// Properties related to the job type
        /// </summary>
        Dictionary<string, object>? JobTypeProperties { get; }
    }


    /// <summary>
    /// Published when a job completes (separately from <see cref="JobCompleted" />)
    /// </summary>
    public interface JobCompleted<out T>
        where T : class
    {
        Guid JobId { get; }

        DateTime Timestamp { get; }

        TimeSpan Duration { get; }

        T Job { get; }

        /// <summary>
        /// Properties specified for this job
        /// </summary>
        Dictionary<string, object>? JobProperties { get; }

        /// <summary>
        /// Properties of the instance that completed the job
        /// </summary>
        Dictionary<string, object>? InstanceProperties { get; }

        /// <summary>
        /// Properties related to the job type
        /// </summary>
        Dictionary<string, object>? JobTypeProperties { get; }
    }
}
