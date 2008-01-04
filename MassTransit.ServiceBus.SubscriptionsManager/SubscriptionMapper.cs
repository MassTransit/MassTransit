namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;
    using Subscriptions;

    public class SubscriptionMapper
    {
        public static Subscription MapFrom(SubscriptionMessage message)
        {
            return new Subscription(message.Address, message.MessageType.FullName);
        }

        public static SubscriptionMessage MapFrom(Subscription subscription)
        {
            return new SubscriptionMessage(Type.GetType(subscription.Message), subscription.Address, SubscriptionMessage.SubscriptionChangeType.Add);
        }

        public static List<SubscriptionMessage> MapFrom(List<Subscription> subscriptions)
        {
            List<SubscriptionMessage> result = new List<SubscriptionMessage>();
            subscriptions.ForEach(delegate(Subscription subscription)
                                     {
                                         result.Add(MapFrom(subscription));
                                     });
            return result;
        }
    }
}