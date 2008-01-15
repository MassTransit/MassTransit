namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using Subscriptions;

    public class SubscriptionMapper
    {
        public static StoredSubscription MapFrom(SubscriptionChange message)
        {
            return new StoredSubscription(message.Subscription.Address.ToString(), message.Subscription.MessageName);
        }

        public static Subscription MapFrom(StoredSubscription storedSubscription)
        {
            return new Subscription(new Uri(storedSubscription.Address), storedSubscription.Message);
        }

        public static List<Subscription> MapFrom(IList<StoredSubscription> subscriptions)
        {
            List<Subscription> result = new List<Subscription>();
            
            foreach (StoredSubscription storedSubscription in subscriptions)
            {
                result.Add(MapFrom(storedSubscription));
            }

            return result;
        }
    }
}