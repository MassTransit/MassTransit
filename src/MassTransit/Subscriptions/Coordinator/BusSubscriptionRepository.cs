// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Subscriptions.Coordinator
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using Messages;
    using Stact;

    public class BusSubscriptionRepository :
        SubscriptionRepository
    {
        static readonly ILog _log = Logger.Get(typeof(BusSubscriptionRepository));

        readonly Fiber _fiber;
        readonly Uri _busUri;
        readonly SubscriptionStorage _storage;
        bool _disposed;

        public BusSubscriptionRepository(Uri busUri, SubscriptionStorage storage)
        {
            string uri = busUri.AbsoluteUri;
            if (busUri.Query.Length > 0)
                uri = uri.Replace(busUri.Query, "");

            _busUri = new Uri(uri);
            _storage = storage;
            _fiber = new PoolFiber();
        }

        public void Add(Guid peerId, Uri peerUri, Guid subscriptionId, Uri endpointUri, string messageName, string correlationId)
        {
            var subscription = new PersistentSubscription(_busUri, peerId, peerUri, subscriptionId, endpointUri, messageName,
                correlationId);

            _fiber.Add(() => Add(subscription));
        }

        public void Remove(Guid peerId, Uri peerUri, Guid subscriptionId, Uri endpointUri, string messageName, string correlationId)
        {
            var subscription = new PersistentSubscription(_busUri, peerId, peerUri, subscriptionId, endpointUri, messageName,
                correlationId);

            _fiber.Add(() => Remove(subscription));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Load(SubscriptionRouter router)
        {
            _fiber.Add(() => LoadSubscriptions(router));
        }

        void LoadSubscriptions(SubscriptionRouter router)
        {
            int messageNumber = 1;
            try
            {
                IEnumerable<PersistentSubscription> existing = _storage.Load(_busUri);

                var knownPeers = new HashSet<Guid>();

                foreach (PersistentSubscription subscription in existing)
                {
                    if (!knownPeers.Contains(subscription.PeerId))
                    {
                        _log.DebugFormat("Loading peer: {0} {1}", subscription.PeerId, subscription.PeerUri);

                        router.Send(new AddPeerMessage
                            {
                                PeerId = subscription.PeerId,
                                PeerUri = subscription.PeerUri,
                                Timestamp = subscription.Updated.Ticks,
                            });
                    }

                    _log.DebugFormat("Loading subscription: {0}", subscription);

                    router.Send(new AddPeerSubscriptionMessage
                        {
                            PeerId = subscription.PeerId,
                            SubscriptionId = subscription.SubscriptionId,
                            EndpointUri = subscription.EndpointUri,
                            MessageName = subscription.MessageName,
                            CorrelationId = subscription.CorrelationId,
                            MessageNumber = messageNumber++,
                        });
                }
            }
            catch (Exception ex)
            {
                _log.Error("Failed to load existing subscriptions", ex);
            }
        }

        void Add(PersistentSubscription subscription)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SubscriptionRepository.Add: {0}, {1}", subscription.MessageName,
                        subscription.SubscriptionId);

                _storage.Add(subscription);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add persistent subscription", ex);
            }
        }

        void Remove(PersistentSubscription subscription)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SubscriptionRepository.Remove: {0}, {1}", subscription.MessageName,
                        subscription.SubscriptionId);

                _storage.Remove(subscription);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to remove persistent subscription", ex);
            }
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _fiber.Shutdown(TimeSpan.FromSeconds(30));
                _storage.Dispose();
            }

            _disposed = true;
        }
    }
}