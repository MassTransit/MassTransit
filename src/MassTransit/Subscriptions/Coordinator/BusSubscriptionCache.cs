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
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using Magnum.Caching;
    using Messages;

    public class BusSubscriptionCache
    {
        static readonly ILog _log = Logger.Get(typeof (BusSubscriptionCache));
        readonly SubscriptionObserver _observer;
        readonly Cache<SubscriptionKey, BusSubscription> _subscriptions;

        public BusSubscriptionCache(SubscriptionObserver observer)
        {
            _observer = observer;
            _subscriptions =
                new ConcurrentCache<SubscriptionKey, BusSubscription>(
                    x => new BusSubscription(x.MessageName, x.CorrelationId, _observer));
        }

        public IEnumerable<Subscription> Subscriptions
        {
            get { return _subscriptions.SelectMany(x => x.Subscriptions).ToList(); }
        }

        public void OnSubscribeTo(SubscribeTo message)
        {
            var key = new SubscriptionKey(message.MessageName, message.CorrelationId);

            BusSubscription subscription = _subscriptions.Get(key);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("SubscribeTo: {0}, {1}", message.MessageName, message.SubscriptionId);

            subscription.OnSubscribeTo(message);
        }

        public void OnUnsubscribeFrom(UnsubscribeFrom message)
        {
            var key = new SubscriptionKey(message.MessageName, message.CorrelationId);

            _subscriptions.WithValue(key, subscription =>
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("UnsubscribeFrom: {0}, {1}", message.MessageName, message.SubscriptionId);

                    subscription.OnUnsubscribeFrom(message);
                });
        }
    }
}