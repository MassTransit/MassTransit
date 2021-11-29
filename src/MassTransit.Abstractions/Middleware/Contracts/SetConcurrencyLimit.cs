#nullable enable
namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Sets the concurrency limit of a concurrency limit filter
    /// </summary>
    public interface SetConcurrencyLimit
    {
        /// <summary>
        /// The timestamp at which the adjustment command was sent
        /// </summary>
        DateTime? Timestamp { get; }

        /// <summary>
        /// The identifier of the concurrency limit to set (optional)
        /// </summary>
        string? Id { get; }

        /// <summary>
        /// The new concurrency limit for the filter
        /// </summary>
        int ConcurrencyLimit { get; }
    }
}
