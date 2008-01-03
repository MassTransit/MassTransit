namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using Messages;
    using Subscriptions;

    public class SubscriptionServiceBus : ServiceBus
    {
        public SubscriptionServiceBus(IEndpoint endpoint, ISubscriptionStorage subscriptionStorage) : base(endpoint, subscriptionStorage)
        {
            this.MessageEndpoint<SubscriptionMessage>().Subscribe(OnSubscriptionMessageReceived);
            this.MessageEndpoint<RequestCacheUpdate>().Subscribe(OnRequestCacheUpdate);
        }


        public void OnSubscriptionMessageReceived(MessageContext<SubscriptionMessage> ctx)
        {
            // Add / Remove Subscription to Repository

            // Publish it so others get it?
            this.Publish(ctx.Message);
        }

        public void OnRequestCacheUpdate(MessageContext<RequestCacheUpdate> ctx)
        {
            //return a complete list of SubscriptionMessages
            ctx.Reply(new CacheUpdateResponse());
        }
    }
}