namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    public class SubscriptionMapper
    {
        public static Subscription MapFrom(SubscriptionMessage message)
        {
            return new Subscription(message.Address.AbsolutePath, message.MessageName);
        }

        public static SubscriptionMessage MapFrom(Subscription subscription)
        {
            return new SubscriptionMessage(Type.GetType(subscription.Message), new Uri(subscription.Address), SubscriptionMessage.SubscriptionChangeType.Add);
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