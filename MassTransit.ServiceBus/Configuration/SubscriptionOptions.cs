namespace MassTransit.ServiceBus.Configuration
{
    using System;

    public class SubscriptionOptions
    {
        public Uri Address { get; set; }
        public Type SubscriptionStore { get; set; }
    }
}