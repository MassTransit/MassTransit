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
    using System.Linq;
    using Logging;
    using Magnum.Extensions;
    using Messages;

    public class EndpointSubscription
    {
        static readonly ILog _log = Logger.Get(typeof(EndpointSubscription));
        readonly string _correlationId;
        readonly IDictionary<Guid, PeerSubscription> _ids;
        readonly string _messageName;
        readonly SubscriptionObserver _observer;
        readonly Uri _peerUri;
        readonly SubscriptionRepository _repository;
        Uri _endpointUri;
        Guid _subscriptionId;

        public EndpointSubscription(Uri peerUri, string messageName, string correlationId,
            SubscriptionObserver observer, SubscriptionRepository repository)
        {
            _peerUri = peerUri;
            _messageName = messageName;
            _correlationId = correlationId;
            _observer = observer;
            _repository = repository;

            _ids = new Dictionary<Guid, PeerSubscription>();

            _subscriptionId = Guid.Empty;
        }

        public void Send(AddPeerSubscription message)
        {
            if (_ids.ContainsKey(message.SubscriptionId))
                return;

            _ids.Add(message.SubscriptionId, message);

            _repository.Add(message.PeerId, _peerUri, message.SubscriptionId, message.EndpointUri, message.MessageName,
                message.CorrelationId);

            if (_ids.Count > 1)
                return;

            _subscriptionId = NewId.NextGuid();
            _endpointUri = message.EndpointUri;

            var add = new SubscriptionAddedMessage
                {
                    SubscriptionId = _subscriptionId,
                    EndpointUri = _endpointUri,
                    MessageName = _messageName,
                    CorrelationId = _correlationId,
                };

            _log.DebugFormat("PeerSubscriptionAdded: {0}, {1} {2}", _messageName, _endpointUri, _subscriptionId);

            _observer.OnSubscriptionAdded(add);
        }

        public void Send(RemovePeerSubscription message)
        {
            bool wasRemoved = _ids.Remove(message.SubscriptionId);
            if (!wasRemoved)
                return;

            RemoveSubscriptions(message.PeerId, Enumerable.Repeat(message.SubscriptionId, 1));

            if (_ids.Count != 0)
                return;

            NotifyRemoveSubscription();
        }

        public void Send(AddPeer message)
        {
            List<KeyValuePair<Guid, PeerSubscription>> remove =
                _ids.Where(x => x.Value.PeerId != message.PeerId).ToList();

            remove.Each(kv => { RemoveSubscriptions(kv.Key, Enumerable.Repeat(kv.Value.SubscriptionId, 1)); });

            if (_ids.Count == 0 && _subscriptionId != Guid.Empty)
            {
                _log.DebugFormat("Removing expired subscription for {0} {1}", _messageName, _endpointUri);

                NotifyRemoveSubscription();
            }
        }

        public void Send(RemovePeer message)
        {
            List<Guid> remove = _ids.Where(x => x.Value.PeerId == message.PeerId)
                .Select(x => x.Key).ToList();

            RemoveSubscriptions(message.PeerId, remove);
        }

        void RemoveSubscriptions(Guid peerId, IEnumerable<Guid> remove)
        {
            int count = 0;
            remove.Each(subscriptionId =>
                {
                    _ids.Remove(subscriptionId);

                    _repository.Remove(peerId, _peerUri, subscriptionId, _endpointUri, _messageName, _correlationId);
                    count++;
                });

            _log.DebugFormat("Removed {0} subscriptions for peer: {1} {2}", count, peerId, _endpointUri);
        }

        void NotifyRemoveSubscription()
        {
            var remove = new SubscriptionRemovedMessage
                {
                    SubscriptionId = _subscriptionId,
                    EndpointUri = _endpointUri,
                    MessageName = _messageName,
                    CorrelationId = _correlationId,
                };

            _log.DebugFormat("PeerSubscriptionRemoved: {0}, {1} {2}", _messageName, _endpointUri, _subscriptionId);

            _observer.OnSubscriptionRemoved(remove);

            _subscriptionId = Guid.Empty;
        }
    }
}