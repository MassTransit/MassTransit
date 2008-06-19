namespace MassTransit.ServiceBus.Subscriptions.ClientHandlers
{
    using Messages;

    public class RemoveSubscriptionHandler :
        Consumes<RemoveSubscription>.All
    {
        private readonly ISubscriptionCache _cache;
        private readonly IServiceBus _bus;

        public RemoveSubscriptionHandler(ISubscriptionCache cache, IServiceBus bus)
        {
            _cache = cache;
            _bus = bus;
        }

        public void Consume(RemoveSubscription message)
        {
            if (!ClientUtil.IsOwnedSubscription(message.Subscription, _bus))
                _cache.Remove(message.Subscription);
        }
    }
}