namespace MassTransit.ServiceBus.Subscriptions.ClientHandlers
{
    using Messages;

    public class CacheUpdateHandler :
        Consumes<CacheUpdateResponse>.All
    {
        private readonly ISubscriptionCache _cache;
        private readonly IServiceBus _bus;

        public CacheUpdateHandler(ISubscriptionCache cache, IServiceBus bus)
        {
            _cache = cache;
            _bus = bus;
        }

        public void Consume(CacheUpdateResponse message)
        {

            foreach (Subscription sub in message.Subscriptions)
            {
                if (!ClientUtil.IsOwnedSubscription(sub, _bus))
                    _cache.Add(sub);
            }
        }

        
    }
}