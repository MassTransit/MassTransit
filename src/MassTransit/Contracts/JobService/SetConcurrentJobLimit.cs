namespace MassTransit.Contracts.JobService
{
    using System;


    /// <summary>
    /// Set the limit for concurrent consumers of the specified type
    /// </summary>
    public interface SetConcurrentJobLimit
    {
        Guid JobTypeId { get; }

        int ConcurrentJobLimit { get; }

        ConcurrentLimitKind Kind { get; }

        /// <summary>
        /// How long a overridden limit should be in effect
        /// </summary>
        TimeSpan? Duration { get; }
    }
}
