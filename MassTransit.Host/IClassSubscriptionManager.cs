using System;
using System.Collections.Generic;
using MassTransit.ServiceBus;

namespace MassTransit.Host
{
    public interface IClassSubscriptionManager
    {
        IList<Type> FindMessageTypes(Type classType);
        void SubscribeHandlers(IServiceBus bus, IAutoSubscriber autoSubscriber);
    }
}