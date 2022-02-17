namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a routing slip is revised during execution
    /// </summary>
    public interface RoutingSlipRevised
    {
        /// <summary>
        /// The tracking number of the routing slip that completed
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The execution that modified the routing slip
        /// </summary>
        Guid ExecutionId { get; }

        /// <summary>
        /// The date/time when the routing slip completed
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The time from when the routing slip was created until the completion
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The name of the activity that terminated the routing slip
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        /// The host that executed the activity
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The variables that were present once the routing slip completed, can be used
        /// to capture the output of the slip - real events should likely be used for real
        /// completion items but this is useful for some cases
        /// </summary>
        IDictionary<string, object> Variables { get; }

        /// <summary>
        /// The new itinerary for the routing slip
        /// </summary>
        Activity[] Itinerary { get; }

        /// <summary>
        /// The previous itinerary of the routing slip that is no longer included
        /// </summary>
        Activity[] DiscardedItinerary { get; }
    }
}
