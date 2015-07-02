namespace MassTransit.Steward.Contracts
{
    using System;


    /// <summary>
    /// An unavailable resource, including the reason the resource is unavailable
    /// </summary>
    public interface UnavailableResource
    {
        /// <summary>
        /// The resource identifier
        /// </summary>
        Uri Resource { get; }

        /// <summary>
        /// The timestamp at which the resource was reported as unavailable
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// The reason the resource is unavailable (can be anything, but an exception message is commonly used)
        /// </summary>
        string Reason { get; }
    }
}