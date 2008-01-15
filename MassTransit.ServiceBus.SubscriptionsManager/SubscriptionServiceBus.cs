using System;
using MassTransit.ServiceBus.Subscriptions.Messages;

namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System.Collections.Generic;
    using Subscriptions;

    public class SubscriptionServiceBus : ServiceBus
    {
        public SubscriptionServiceBus(IEndpoint endpoint, ISubscriptionStorage subscriptionStorage)
            : base(endpoint, subscriptionStorage)
        {
            this.Subscribe<RequestCacheUpdate>(OnRequestCacheUpdate);
            this.Subscribe<SubscriptionChange>(OnSubscriptionMessageReceived);
            this.Subscribe<RequestCacheUpdateForMessage>(OnRequestSubscribersForMessage);
        }


        public void OnSubscriptionMessageReceived(MessageContext<SubscriptionChange> ctx)
        {
            RegisterSenderForUpdates(ctx.Envelope);

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
            RegisterSenderForUpdates(ctx.Envelope);

            //return a complete list of SubscriptionMessages
            List<SubscriptionChange> result = new List<SubscriptionChange>();
            foreach (Subscription subscription in base.SubscriptionStorage.List())
            {
                result.Add(new SubscriptionChange(subscription, SubscriptionChangeType.Add));
            }
            ctx.Reply(new CacheUpdateResponse(result));
        }


        public void OnRequestSubscribersForMessage(MessageContext<RequestCacheUpdateForMessage> ctx)
        {
            RegisterSenderForUpdates(ctx.Envelope);

            //return a complete list of SubscriptionMessages
            List<SubscriptionChange> result = new List<SubscriptionChange>();
            foreach (Subscription subscription in base.SubscriptionStorage.List(ctx.Message.MessageName))
            {
                result.Add(new SubscriptionChange(subscription, SubscriptionChangeType.Add));
            }
            ctx.Reply(new CacheUpdateResponse(result));
        }

        public void RegisterSenderForUpdates(IEnvelope env)
        {
            //This is basically setting anybody that talks to us up for updates
            base.SubscriptionStorage.Add(typeof(CacheUpdateResponse).FullName, env.ReturnEndpoint.Uri);
        }
    }
}