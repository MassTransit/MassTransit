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

namespace MassTransit.ServiceBus.SubscriptionsManager.Client
{
    using System;
    using log4net;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using Subscriptions;

    /// <summary>
    /// Works with the remote subscription storage to update local subscriptions with the other endpoints.
    /// </summary>
    public class ClientProxy
    {
        private IEndpoint _wellKnownSubscriptionManagerEndpoint;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ClientProxy));
        private IServiceBus _bus;
        private ISubscriptionStorage _storage;

        public ClientProxy(IEndpoint wellKnownSubscriptionManagerEndpoint)
        {
            _wellKnownSubscriptionManagerEndpoint = wellKnownSubscriptionManagerEndpoint;
        }

        public void StartWatching(IServiceBus bus, ISubscriptionStorage storage)
        {
            if(_log.IsDebugEnabled)
                _log.DebugFormat("Subscription Manager Client Started");

            _bus = bus;
            _bus.Subscribe<CacheUpdateResponse>(RespondToCacheUpdateMessage);
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, new RequestCacheUpdate());

            _storage = storage;
            _storage.SubscriptionChanged += storage_SubscriptionChanged;

            foreach (Subscription subscription in _storage.List())
            {
                SendUpdate(new SubscriptionChange(subscription, SubscriptionChangeType.Add));
            }
        }

        private void storage_SubscriptionChanged(object sender, SubscriptionChangedEventArgs e)
        {
            //send stuff to the wellknown endpoint
            SendUpdate(e.Change);
            
        }

        public void SendUpdate(SubscriptionChange change)
        {
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, change);
        }

        public void RespondToCacheUpdateMessage(IMessageContext<CacheUpdateResponse> ctx)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Received Update from {0}", ctx.Envelope.ReturnEndpoint);

            foreach (SubscriptionChange change in ctx.Message.Subscriptions)
            {
                switch(change.ChangeType)
                {
                    case SubscriptionChangeType.Add:
                        _storage.Add(change.Subscription.MessageName, change.Subscription.Address);
                        break;
                    case SubscriptionChangeType.Remove:
                        _storage.Remove(change.Subscription.MessageName, change.Subscription.Address);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
