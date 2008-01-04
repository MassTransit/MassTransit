namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;

    public interface ISubscriptionRepository
    {
        void Add(Subscription subscription);
        void Deactivate(Subscription subscription);
        List<Subscription> List();
        List<Subscription> List(Type message);
    }
}