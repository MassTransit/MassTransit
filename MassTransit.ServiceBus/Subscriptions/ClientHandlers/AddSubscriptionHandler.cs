namespace MassTransit.ServiceBus.Subscriptions.ClientHandlers
{
    using Messages;

    public class AddSubscriptionHandler :
        Consumes<AddSubscription>.All
    {
        private readonly ISubscriptionCache _cache;


        public AddSubscriptionHandler(ISubscriptionCache cache)
        {
            _cache = cache;
        }

        public void Consume(AddSubscription message)
        {
            if (!IsOwnedSubscription(message.Subscription))
                _cache.Add(message.Subscription);
        }
    }
}