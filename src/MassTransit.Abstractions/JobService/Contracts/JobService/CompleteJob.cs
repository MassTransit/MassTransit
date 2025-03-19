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
        /// The JobTypeId, to ensure the proper job type is started
        /// </summary>
        Guid JobTypeId { get; }

        Dictionary<string, object>? JobProperties { get; }

        Dictionary<string, object>? InstanceProperties { get; }

        Dictionary<string, object>? JobTypeProperties { get; }
    }
}
