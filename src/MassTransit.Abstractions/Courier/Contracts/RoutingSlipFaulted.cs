namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a routing slip faults (after compensation)
    /// </summary>
    public interface RoutingSlipFaulted
    {
        /// <summary>
        /// The tracking number of the routing slip that faulted
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The date/time when the routing slip faulted
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The time from when the routing slip was created until the fault occurred
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The exception information from the faulting activities
        /// </summary>
        ActivityException[] ActivityExceptions { get; }

        /// <summary>
        /// The variables that were present once the routing slip completed, can be used
        /// to capture the output of the slip - real events should likely be used for real
        /// completion items but this is useful for some cases
        /// </summary>
        IDictionary<string, object> Variables { get; }
    }
}
