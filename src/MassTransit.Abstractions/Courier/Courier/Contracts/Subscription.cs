namespace MassTransit.Courier.Contracts
{
    using System;
    using Serialization;


    /// <summary>
    /// A routing slip subscription defines a specific endpoint where routing
    /// slip events should be sent (not published). If specified, events are not published.
    /// </summary>
    public interface Subscription
    {
        /// <summary>
        /// The address where events should be sent
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// The events that are subscribed
        /// </summary>
        RoutingSlipEvents Events { get; }

        /// <summary>
        /// The event contents to include when published
        /// </summary>
        RoutingSlipEventContents Include { get; }

        /// <summary>
        /// If specified, events are only used in this subscription if the activity name matches
        /// </summary>
        string? ActivityName { get; }

        /// <summary>
        /// The message sent as part of the subscription
        /// </summary>
        MessageEnvelope? Message { get; }
    }
}
