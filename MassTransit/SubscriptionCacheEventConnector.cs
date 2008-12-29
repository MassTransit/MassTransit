namespace MassTransit
{
    using System;
    using Pipeline;
    using Subscriptions;

    public class SubscriptionCacheEventConnector :
        ISubscriptionEvent
    {
        private readonly ISubscriptionCache _cache;
        private readonly IEndpoint _endpoint;

        public SubscriptionCacheEventConnector(ISubscriptionCache cache, IEndpoint endpoint)
        {
            _cache = cache;
            _endpoint = endpoint;
        }

        public UnsubscribeAction SubscribedTo(Type messageType)
        {
            Subscription subscription = new Subscription(messageType, _endpoint.Uri);

            _cache.Add(subscription);

            return () =>
                       {
                           _cache.Remove(subscription);
                           return true;
                       };
        }

        public UnsubscribeAction SubscribedTo(Type messageType, string correlationId)
        {
            Subscription subscription = new Subscription(messageType, correlationId, _endpoint.Uri);

            _cache.Add(subscription);

            return () =>
                       {
                           _cache.Remove(subscription);
                           return true;
                       };
        }

        public void UnsubscribedFrom(Type messageType)
        {
            Subscription subscription = new Subscription(messageType, _endpoint.Uri);

            _cache.Remove(subscription);
        }

        public void UnsubscribedFrom(Type messageType, string correlationId)
        {
            Subscription subscription = new Subscription(messageType, correlationId, _endpoint.Uri);

            _cache.Remove(subscription);
        }
    }
}