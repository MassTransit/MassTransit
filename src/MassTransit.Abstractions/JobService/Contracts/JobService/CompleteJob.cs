namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    [ConfigureConsumeTopology(false)]
    public interface CompleteJob
    {
        Guid JobId { get; }

        DateTime Timestamp { get; }

        TimeSpan Duration { get; }

        /// <summary>
        /// The job, as an object dictionary
        /// </summary>
        Dictionary<string, object> Job { get; }

        /// <summary>
        /// The result of the job
        /// </summary>
        Dictionary<string, object> Result { get; }

        /// <summary>
        /// The JobTypeId, to ensure the proper job type is started
        /// </summary>
        Guid JobTypeId { get; }
    }
}
