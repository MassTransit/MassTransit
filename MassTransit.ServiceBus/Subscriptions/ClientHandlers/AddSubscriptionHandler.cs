namespace MassTransit.ServiceBus.Subscriptions.ClientHandlers
{
    using Messages;

    public class AddSubscriptionHandler :
        Consumes<AddSubscription>.All
    {
        private readonly ISubscriptionCache _cache;
        private readonly IServiceBus _bus;

        public AddSubscriptionHandler(ISubscriptionCache cache, IServiceBus bus)
        {
            _cache = cache;
            _bus = bus;
        }

        public void Consume(AddSubscription message)
        {
            if (!ClientUtil.IsOwnedSubscription(message.Subscription, _bus))
                _cache.Add(message.Subscription);
        }
    }
}