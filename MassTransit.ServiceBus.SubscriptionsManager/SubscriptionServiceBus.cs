using System;
using MassTransit.ServiceBus.Subscriptions.Messages;

namespace MassTransit.ServiceBus.SubscriptionsManager
{
    public class SubscriptionServiceBus : ServiceBus
    {
        public SubscriptionServiceBus(IEndpoint endpoint, ISubscriptionStorage subscriptionStorage)
            : base(endpoint, subscriptionStorage)
        {
            this.Subscribe<SubscriptionChange>(OnSubscriptionMessageReceived);
            this.Subscribe<RequestCacheUpdate>(OnRequestCacheUpdate);
            this.Subscribe<RequestCacheUpdateForMessage>(OnRequestSubscribersForMessage);
        }


        public void OnSubscriptionMessageReceived(MessageContext<SubscriptionChange> ctx)
        {
            RegisterForUpdates(ctx.Envelope);

            // Add / Remove Subscription to Repository
            switch(ctx.Message.ChangeType)
            {
                case SubscriptionChangeType.Add:
                    base.SubscriptionStorage.Add(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
                    break;
                case SubscriptionChangeType.Remove:
                    base.SubscriptionStorage.Remove(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Publish it so others get it?
            this.Publish(ctx.Message);
        }

        public void OnRequestCacheUpdate(MessageContext<RequestCacheUpdate> ctx)
        {
            RegisterForUpdates(ctx.Envelope);

            //return a complete list of SubscriptionMessages
            //ctx.Reply(new CacheUpdateResponse(base.SubscriptionStorage.List()));
        }


        public void OnRequestSubscribersForMessage(MessageContext<RequestCacheUpdateForMessage> ctx)
        {
            RegisterForUpdates(ctx.Envelope);

            //return a complete list of SubscriptionMessages
            //ctx.Reply(new CacheUpdateResponse(this.SubscriptionStorage.List(ctx.Message.MessageName)));
        }

        public void RegisterForUpdates(IEnvelope env)
        {
            //This is basically setting anybody that talks to us up for updates
            base.SubscriptionStorage.Add(typeof(CacheUpdateResponse).FullName, env.ReturnEndpoint.Uri);
        }
    }
}