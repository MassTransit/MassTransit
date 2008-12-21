// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Services.Subscriptions.Client
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Subscriptions;
    using Messages;

    public class LocalSubscriptionCoordinator :
        Consumes<CacheUpdateResponse>.All,
        IDisposable
    {
        private readonly ISubscriptionCache _cache;
        private readonly LocalEndpointHandler _endpointsLocalToThisBus;
        private readonly IEndpoint _wellKnownEndpointForSubscriptionServices;

        public LocalSubscriptionCoordinator(ISubscriptionCache cache, IEndpoint wellKnownEndpointForSubscriptionServices, LocalEndpointHandler endpointsLocalToThisBus)
        {
            _cache = cache;
            _endpointsLocalToThisBus = endpointsLocalToThisBus;

            _cache.OnAddSubscription += WhenASubscriptionIsAdded;
            _cache.OnRemoveSubscription += WhenASubscriptionIsRemoved;

            _wellKnownEndpointForSubscriptionServices = wellKnownEndpointForSubscriptionServices;
        }

        public void Dispose()
        {
            _cache.OnAddSubscription -= WhenASubscriptionIsAdded;
            _cache.OnRemoveSubscription -= WhenASubscriptionIsRemoved;
        }

        public void WhenASubscriptionIsAdded(object sender, SubscriptionEventArgs e)
        {
           if(WeShouldOnlyBroadcastChangesAboutOurEndpoints(e.Subscription))
           {
               AddSubscription message = new AddSubscription(Convert(e.Subscription));

               _wellKnownEndpointForSubscriptionServices.Send(message);
           }
        }


        public void WhenASubscriptionIsRemoved(object sender, SubscriptionEventArgs e)
        {
            if (WeShouldOnlyBroadcastChangesAboutOurEndpoints(e.Subscription))
            {
                RemoveSubscription message = new RemoveSubscription(Convert(e.Subscription));

                _wellKnownEndpointForSubscriptionServices.Send(message);
            } 
        }

        private bool WeShouldOnlyBroadcastChangesAboutOurEndpoints(Subscription subscription)
        {
            return _endpointsLocalToThisBus.ContainsEndpoint(subscription.EndpointUri);
        }

        public void Consume(CacheUpdateResponse message)
        {
            // to make things good, we need to enumerate the local subscriptions 
            // and add anything that is local in case it was missed
            // during startup

            PublishLocalCacheToServer();
        }

        private void PublishLocalCacheToServer()
        {
            IList<Subscription> subscriptions = _cache.List();
            foreach (Subscription subscription in subscriptions)
            {
                if (WeShouldOnlyBroadcastChangesAboutOurEndpoints(subscription))
                {
                    AddSubscription message = new AddSubscription(Convert(subscription));

                    _wellKnownEndpointForSubscriptionServices.Send(message);
                }
            }
        }

        private SubscriptionInformation Convert(Subscription subscription)
        {
            return new SubscriptionInformation(subscription.MessageName, subscription.CorrelationId, subscription.EndpointUri);
        }
    }
}