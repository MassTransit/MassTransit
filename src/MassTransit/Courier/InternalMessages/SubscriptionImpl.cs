namespace MassTransit.Courier.InternalMessages
{
    using System;
    using Contracts;
    using MassTransit.Serialization;


    public class SubscriptionImpl :
        Subscription
    {
        protected SubscriptionImpl()
        {
        }

        public SubscriptionImpl(Uri address, RoutingSlipEvents events, RoutingSlipEventContents include, string activityName = null,
            MessageEnvelope message = null)
        {
            Include = include;
            ActivityName = activityName;
            Address = address;
            Events = events;
            Message = message;
        }

        public Uri Address { get; private set; }
        public RoutingSlipEvents Events { get; private set; }
        public RoutingSlipEventContents Include { get; private set; }
        public MessageEnvelope Message { get; private set; }
        public string ActivityName { get; private set; }
    }
}
