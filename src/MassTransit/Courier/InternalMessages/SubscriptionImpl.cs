namespace MassTransit.Courier.InternalMessages
{
    using System;
    using Contracts;
    using MassTransit.Serialization;


    [Serializable]
    public class SubscriptionImpl :
        Subscription
    {
        public SubscriptionImpl()
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

        public Uri Address { get; set; }
        public RoutingSlipEvents Events { get; set; }
        public RoutingSlipEventContents Include { get; set; }
        public MessageEnvelope Message { get; set; }
        public string ActivityName { get; set; }
    }
}
