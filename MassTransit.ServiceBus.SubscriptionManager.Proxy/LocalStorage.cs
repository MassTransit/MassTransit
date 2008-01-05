namespace MassTransit.ServiceBus.SubscriptionManager.Proxy
{
    using System;
    using System.Collections.Generic;
    using Subscriptions;

    public class LocalStorage : ISubscriptionStorage
    {
        private IServiceBus _bus;
        private IEndpoint _wellKnownSubscriptionManagerEndpoint;
        


        public LocalStorage(IServiceBus bus, IEndpoint wellKnownSubscriptionManagerEndpoint)
        {
            _bus = bus;
            _wellKnownSubscriptionManagerEndpoint = wellKnownSubscriptionManagerEndpoint;
        }

        public IList<IEndpoint> List<T>(params T[] messages) where T : IMessage
        {
            IList<IEndpoint> result = new List<IEndpoint>();



            return result;
        }

        public void Add(Type messageType, IEndpoint endpoint)
        {
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, new SubscriptionMessage(messageType, endpoint.Address, SubscriptionMessage.SubscriptionChangeType.Add));
        }

        public void Remove(Type messageType, IEndpoint endpoint)
        {
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, new SubscriptionMessage(messageType, endpoint.Address, SubscriptionMessage.SubscriptionChangeType.Remove));
        }

        public void Dispose()
        {
            _wellKnownSubscriptionManagerEndpoint.Dispose();
        }
    }
}