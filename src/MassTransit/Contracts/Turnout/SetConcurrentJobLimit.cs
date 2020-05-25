namespace MassTransit.Contracts.Turnout
{
    using System;


    public interface SetConcurrentJobLimit
    {
        Guid JobTypeId { get; }

        int ConcurrentJobLimit { get; }

        JobLimitKind Kind { get; }

        /// <summary>
        /// How long a overridden limit should be in effect
        /// </summary>
        TimeSpan? Duration { get; }
    }
}
