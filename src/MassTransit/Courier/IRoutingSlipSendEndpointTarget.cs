namespace MassTransit.Courier
{
    using System;
    using Contracts;
    using MassTransit.Serialization;


    public interface IRoutingSlipSendEndpointTarget
    {
        /// <summary>
        /// Adds a custom subscription message to the routing slip which is sent at the specified events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        /// <param name="contents">The contents of the routing slip event</param>
        /// <param name="activityName"></param>
        /// <param name="message">The custom message to be sent</param>
        void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName, MessageEnvelope message);
    }
}
