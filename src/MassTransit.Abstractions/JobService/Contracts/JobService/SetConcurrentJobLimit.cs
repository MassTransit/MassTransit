namespace MassTransit.Contracts.JobService
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// When the bus is started, the current job limit for a job type is published along with the instance address.
    /// </summary>
    public interface SetConcurrentJobLimit
    {
        Guid JobTypeId { get; }

        Uri InstanceAddress { get; }

        int ConcurrentJobLimit { get; }

        ConcurrentLimitKind Kind { get; }

        /// <summary>
        /// How long a overridden limit should be in effect
        /// </summary>
        TimeSpan? Duration { get; }

        /// <summary>
        /// If present, the job type name
        /// </summary>
        string? JobTypeName { get; }

        /// <summary>
        /// Allows properties to be submitted by the job service instance that can be used by the job distribution strategy
        /// </summary>
        Dictionary<string, object>? JobTypeProperties { get; }

        /// <summary>
        /// Allows properties to be submitted by the job service instance that can be used by the job distribution strategy
        /// </summary>
        Dictionary<string, object>? InstanceProperties { get; }

        /// <summary>
        /// If configured, specifies a global limit across all job consumer instances
        /// </summary>
        int? GlobalConcurrentJobLimit { get; }
    }
}
