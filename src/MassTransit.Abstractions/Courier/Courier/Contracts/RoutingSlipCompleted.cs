namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a routing slip completes
    /// </summary>
    public interface RoutingSlipCompleted
    {
        /// <summary>
        /// The tracking number of the routing slip that completed
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The date/time when the routing slip completed
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The time from when the routing slip was created until the completion
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The variables that were present once the routing slip completed, can be used
        /// to capture the output of the slip - real events should likely be used for real
        /// completion items but this is useful for some cases
        /// </summary>
        IDictionary<string, object> Variables { get; }
    }
}
