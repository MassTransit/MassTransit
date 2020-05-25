namespace MassTransit.Contracts.Turnout
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
        IDictionary<string, object> Job { get; }

        /// <summary>
        /// The result of the job
        /// </summary>
        IDictionary<string, object> Result { get; }
    }
}
