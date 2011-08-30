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
    using Magnum.Extensions;
    using Messages;
    using log4net;

    public class BusSubscriptionCache
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (BusSubscriptionCache));
        readonly object _lock = new object();
        readonly SubscriptionObserver _observer;
        readonly IDictionary<SubscriptionKey, BusSubscription> _subscriptions;

        public BusSubscriptionCache(SubscriptionObserver observer)
        {
            _observer = observer;
            _subscriptions = new Dictionary<SubscriptionKey, BusSubscription>();
        }

        public IEnumerable<Subscription> Subscriptions
        {
            get
            {
                lock (_lock)
                    return _subscriptions.Values.SelectMany(x => x.Subscriptions).ToList();
            }
        }

        public void OnSubscribeTo(SubscribeTo message)
        {
            var key = new SubscriptionKey
                {
                    MessageName = message.MessageName,
                    CorrelationId = message.CorrelationId,
                };

            BusSubscription subscription;
            lock (_lock)
            {
                subscription = _subscriptions.Retrieve(key,
                    () => new BusSubscription(message.MessageName, message.CorrelationId, _observer));
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("SubscribeTo: {0}, {1}", message.MessageName, message.SubscriptionId);

            subscription.OnSubscribeTo(message);
        }

        public void OnUnsubscribeFrom(UnsubscribeFrom message)
        {
            var key = new SubscriptionKey
                {
                    MessageName = message.MessageName,
                    CorrelationId = message.CorrelationId,
                };

            BusSubscription subscription;
            bool result;
            lock (_lock)
                result = _subscriptions.TryGetValue(key, out subscription);
            if (result)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("UnsubscribeFrom: {0}, {1}", message.MessageName, message.SubscriptionId);

                subscription.OnUnsubscribeFrom(message);
            }
            else
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("UnsubscribeFrom(unknown): {0}, {1}", message.MessageName, message.SubscriptionId);
            }
        }
    }
}