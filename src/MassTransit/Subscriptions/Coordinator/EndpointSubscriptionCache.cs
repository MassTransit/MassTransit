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
    using Logging;
    using Magnum.Caching;
    using Messages;

    public class EndpointSubscriptionCache
    {
        static readonly ILog _log = Logger.Get(typeof(EndpointSubscriptionCache));

        readonly Cache<SubscriptionKey, EndpointSubscription> _messageSubscriptions;
        readonly SubscriptionObserver _observer;
        readonly Uri _peerUri;

        public EndpointSubscriptionCache(SubscriptionObserver observer, Uri peerUri,
            SubscriptionRepository repository)
        {
            _observer = observer;
            _peerUri = peerUri;
            _messageSubscriptions =
                new DictionaryCache<SubscriptionKey, EndpointSubscription>(
                    key =>
                    new EndpointSubscription(_peerUri, key.MessageName, key.CorrelationId, _observer,
                        repository));
        }

        public void Send(AddPeerSubscription message)
        {
            var key = new SubscriptionKey(message.MessageName, message.CorrelationId);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("AddPeerSubscription: {0}, {1}", message.MessageName, message.SubscriptionId);

            EndpointSubscription subscription = _messageSubscriptions[key];

            subscription.Send(message);
        }

        public void Send(RemovePeerSubscription message)
        {
            var key = new SubscriptionKey(message.MessageName, message.CorrelationId);

            _messageSubscriptions.WithValue(key, subscription =>
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("RemovePeerSubscription: {0}, {1}", message.MessageName, message.SubscriptionId);

                    subscription.Send(message);
                });
        }

        public void Send(AddPeer message)
        {
            _messageSubscriptions.Each(x => x.Send(message));
        }

        public void Send(RemovePeer message)
        {
            _messageSubscriptions.Each(x => x.Send(message));
        }
    }
}