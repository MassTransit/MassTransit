/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

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
        }


        public void OnSubscriptionMessageReceived(IMessageContext<SubscriptionChange> ctx)
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

        public void OnRequestCacheUpdate(IMessageContext<RequestCacheUpdate> ctx)
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

        public void RegisterSenderForUpdates(IEnvelope env)
        {
            //This is basically setting anybody that talks to us up for updates
            base.SubscriptionStorage.Add(typeof(CacheUpdateResponse).FullName, env.ReturnEndpoint.Uri);
        }
    }
}