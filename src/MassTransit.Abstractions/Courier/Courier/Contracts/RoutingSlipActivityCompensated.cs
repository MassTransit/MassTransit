namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    public interface RoutingSlipActivityCompensated
    {
        /// <summary>
        /// The tracking number of the routing slip that faulted
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The tracking number for completion of the activity
        /// </summary>
        Guid ExecutionId { get; }

        /// <summary>
        /// The date/time when the routing slip compensation was finished
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The duration of the activity execution
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The name of the activity that completed
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        /// The host that executed the activity
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The results of the activity saved for compensation
        /// </summary>
        IDictionary<string, object> Data { get; }

        /// <summary>
        /// The variables that were present once the routing slip completed, can be used
        /// to capture the output of the slip - real events should likely be used for real
        /// completion items but this is useful for some cases
        /// </summary>
        IDictionary<string, object> Variables { get; }
    }
}
