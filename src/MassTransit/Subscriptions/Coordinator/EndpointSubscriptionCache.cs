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
    using Logging;
    using Magnum.Caching;
    using Messages;
    using Stact;

    public class EndpointSubscriptionCache
    {
        static readonly ILog _log = Logger.Get(typeof (EndpointSubscriptionCache));

        readonly Fiber _fiber;
        readonly Cache<SubscriptionKey, EndpointSubscription> _messageSubscriptions;
        readonly SubscriptionObserver _observer;
        readonly Scheduler _scheduler;

        public EndpointSubscriptionCache(Fiber fiber, Scheduler scheduler, SubscriptionObserver observer)
        {
            _fiber = fiber;
            _scheduler = scheduler;
            _observer = observer;
            _messageSubscriptions =
                new DictionaryCache<SubscriptionKey, EndpointSubscription>(
                    key => new EndpointSubscription(_fiber, _scheduler, key.MessageName, key.CorrelationId, _observer));
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