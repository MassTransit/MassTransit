namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Published when the concurrency limit of a filter is updated.
    /// </summary>
    public interface ConcurrencyLimitUpdated
    {
        /// <summary>
        /// The actual time at which the adjustment was applied
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The identifier that was adjusted
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The current concurrency limit value
        /// </summary>
        int ConcurrencyLimit { get; }
    }
}
