namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    public interface RoutingSlipActivityFaulted
    {
        /// <summary>
        /// The tracking number of the routing slip that faulted
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The tracking number of this activity execution
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
        /// The exception information from the faulting activity
        /// </summary>
        ExceptionInfo ExceptionInfo { get; }

        /// <summary>
        /// The arguments that were specified for the activity at execution
        /// </summary>
        IDictionary<string, object> Arguments { get; }

        /// <summary>
        /// The variables that were present once the routing slip completed, can be used
        /// to capture the output of the slip - real events should likely be used for real
        /// completion items but this is useful for some cases
        /// </summary>
        IDictionary<string, object> Variables { get; }
    }
}
