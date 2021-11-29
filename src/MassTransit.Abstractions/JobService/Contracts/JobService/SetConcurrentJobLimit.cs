namespace MassTransit.Contracts.JobService
{
    using System;


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
    }
}
