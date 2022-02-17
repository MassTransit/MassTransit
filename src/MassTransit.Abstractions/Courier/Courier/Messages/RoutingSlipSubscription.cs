namespace MassTransit.Courier.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Contracts;
    using Serialization;


    [Serializable]
    public class RoutingSlipSubscription :
        Subscription
    {
    #pragma warning disable CS8618
        public RoutingSlipSubscription()
    #pragma warning restore CS8618
        {
        }

        public RoutingSlipSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents include, string? activityName = null,
            MessageEnvelope? message = null)
        {
            Include = include;
            ActivityName = activityName;
            Address = address;
            Events = events;
            Message = message;
        }

        public RoutingSlipSubscription(Subscription subscription)
        {
            if (subscription.Address == null)
                throw new SerializationException("A subscription address is required");

            Address = subscription.Address;
            Events = subscription.Events;
            Include = subscription.Include;
            Message = subscription.Message;
            ActivityName = subscription.ActivityName;
        }

        public Uri Address { get; set; }
        public RoutingSlipEvents Events { get; set; }
        public RoutingSlipEventContents Include { get; set; }
        public MessageEnvelope? Message { get; set; }
        public string? ActivityName { get; set; }
    }
}
