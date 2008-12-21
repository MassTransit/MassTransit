namespace MassTransit.Services.Subscriptions.Client
{
    using System;
    using MassTransit.Subscriptions;
    using Messages;

    public class RemoteSubscriptionCoordinator :
		Consumes<CacheUpdateResponse>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All,
        IDisposable
    {
        private readonly LocalEndpointHandler _endpointsLocalToThisBus;
        private readonly ISubscriptionCache _cache;

        public RemoteSubscriptionCoordinator(ISubscriptionCache cache, LocalEndpointHandler endpointsLocalToThisBus)
        {
            _cache = cache;
            _endpointsLocalToThisBus = endpointsLocalToThisBus;
        }

        public void Consume(CacheUpdateResponse message)
        {
            foreach (Subscription sub in message.Subscriptions)
            {
                if (WeShouldOnlyAcceptChangesAboutOtherEndpoints(sub))
                    _cache.Add(sub);
            }

            // to make things good, we need to enumerate the local subscriptions 
            // and add anything that is local in case it was missed
            // during startup

            //PublishLocalCacheToServer();
        }

        public void Consume(AddSubscription message)
        {
            var sub = Convert(message.Subscription);
            if (WeShouldOnlyAcceptChangesAboutOtherEndpoints(sub))
                _cache.Add(sub);
        }

        public void Consume(RemoveSubscription message)
        {
            var sub = Convert(message.Subscription);
            if (WeShouldOnlyAcceptChangesAboutOtherEndpoints(sub))
                _cache.Remove(sub);
        }

        private bool WeShouldOnlyAcceptChangesAboutOtherEndpoints(Subscription subscription)
        {
            return !_endpointsLocalToThisBus.ContainsEndpoint(subscription.EndpointUri);
        }

        public void Dispose()
        {
            
        }

        private Subscription Convert(SubscriptionInformation info)
        {
            return new Subscription(info.MessageName, info.CorrelationId, info.EndpointUri);
        }
    }
}