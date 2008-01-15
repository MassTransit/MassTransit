namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;

    public interface ISubscriptionRepository
    {
        void Add(StoredSubscription storedSubscription);
        void Deactivate(StoredSubscription storedSubscription);
        List<StoredSubscription> List();
        List<StoredSubscription> List(Type message);
    }
}