// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Stact;

    public class EndpointSubscription
    {
        static readonly ILog _log = Logger.Get(typeof (EndpointSubscription));
        readonly string _correlationId;
        readonly Fiber _fiber;
        readonly IDictionary<Guid, PeerSubscription> _ids;
        readonly string _messageName;
        readonly SubscriptionObserver _observer;
        readonly Scheduler _scheduler;
        readonly TimeSpan _unsubscribeTimeout = 4.Seconds();
        Uri _endpointUri;
        Guid _subscriptionId;

        public EndpointSubscription(Fiber fiber, Scheduler scheduler, string messageName, string correlationId,
                                    SubscriptionObserver observer)
        {
            _fiber = fiber;
            _scheduler = scheduler;
            _messageName = messageName;
            _correlationId = correlationId;
            _observer = observer;

            _ids = new Dictionary<Guid, PeerSubscription>();

            _subscriptionId = Guid.Empty;
        }

        public void Send(AddPeerSubscription message)
        {
            if (_ids.ContainsKey(message.SubscriptionId))
                return;

            _ids.Add(message.SubscriptionId, message);

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
            if (!wasRemoved || _ids.Count != 0)
                return;

            NotifyRemoveSubscription();
        }

        public void Send(AddPeer message)
        {
            List<Guid> remove = _ids.Where(x => x.Value.PeerId != message.PeerId)
                .Select(x => x.Key).ToList();

            _log.InfoFormat("Removing {0} subscriptions for {1} {2}", remove.Count, _messageName, _endpointUri);

            if (remove.Count > 0)
            {
                remove.Each(x => _ids.Remove(x));
            }

            if (_ids.Count == 0 && _subscriptionId != Guid.Empty)
            {
                _log.InfoFormat("Removing expired subscription for {0} {1}", _messageName, _endpointUri);

                NotifyRemoveSubscription();
            }
        }

        public void Send(RemovePeer message)
        {
            List<Guid> remove = _ids.Where(x => x.Value.PeerId == message.PeerId)
                .Select(x => x.Key).ToList();

            if (remove.Count > 0)
            {
                remove.Each(x => _ids.Remove(x));

                _log.DebugFormat("Removed {0} subscriptions for peer: {1} {2}", remove.Count, message.PeerId,
                    message.PeerUri);
            }
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