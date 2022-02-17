namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// A RoutingSlip is the transport-level interface that is used to carry the details
    /// of a message routing slip over the network.
    /// </summary>
    public interface RoutingSlip
    {
        /// <summary>
        /// The unique tracking number for this routing slip, used to correlate events
        /// and activities
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The time when the routing slip was created
        /// </summary>
        DateTime CreateTimestamp { get; }

        /// <summary>
        /// The list of activities that are remaining
        /// </summary>
        IList<Activity> Itinerary { get; }

        /// <summary>
        /// The logs of activities that have already been executed
        /// </summary>
        IList<ActivityLog> ActivityLogs { get; }

        /// <summary>
        /// The logs of activities that can be compensated
        /// </summary>
        IList<CompensateLog> CompensateLogs { get; }

        /// <summary>
        /// Variables that are carried with the routing slip for use by any activity
        /// </summary>
        IDictionary<string, object> Variables { get; }

        /// <summary>
        /// A list of exceptions that have occurred during routing slip execution
        /// </summary>
        IList<ActivityException> ActivityExceptions { get; }

        /// <summary>
        /// Subscriptions to routing slip events
        /// </summary>
        IList<Subscription> Subscriptions { get; }
    }
}
