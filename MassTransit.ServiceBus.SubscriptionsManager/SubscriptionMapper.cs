namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    public class SubscriptionMapper
    {
        public static Subscription MapFrom(SubscriptionChange message)
        {
            return new Subscription(message.Subscription.Address.AbsolutePath, message.Subscription.MessageName);
        }

        public static SubscriptionChange MapFrom(Subscription subscription)
        {
            return new SubscriptionChange(Type.GetType(subscription.Message), new Uri(subscription.Address), SubscriptionChangeType.Add);
        }

        public static List<SubscriptionChange> MapFrom(List<Subscription> subscriptions)
        {
            List<SubscriptionChange> result = new List<SubscriptionChange>();
            subscriptions.ForEach(delegate(Subscription subscription)
                                     {
                                         result.Add(MapFrom(subscription));
                                     });
            return result;
        }
    }
}