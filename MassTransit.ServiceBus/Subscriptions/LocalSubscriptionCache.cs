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
namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using log4net;

    public class LocalSubscriptionCache :
        ISubscriptionCache
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (LocalSubscriptionCache));
        private readonly object _changeLock = new object();

        private readonly Dictionary<string, List<SubscriptionCacheEntry>> _correlatedSubscriptions =
            new Dictionary<string, List<SubscriptionCacheEntry>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, List<SubscriptionCacheEntry>> _messageTypeSubscriptions =
            new Dictionary<string, List<SubscriptionCacheEntry>>(StringComparer.InvariantCultureIgnoreCase);

        public IList<Subscription> List()
        {
            List<Subscription> result = new List<Subscription>();

            lock (_changeLock)
            {
                foreach (KeyValuePair<string, List<SubscriptionCacheEntry>> pair in _messageTypeSubscriptions)
                {
                    pair.Value.ForEach(
                        delegate(SubscriptionCacheEntry e)
                            {
                                if (!result.Contains(e.Subscription))
                                    result.Add(e.Subscription);
                            });
                }

                foreach (KeyValuePair<string, List<SubscriptionCacheEntry>> pair in _correlatedSubscriptions)
                {
                    pair.Value.ForEach(
                        delegate(SubscriptionCacheEntry e)
                            {
                                if (!result.Contains(e.Subscription))
                                    result.Add(e.Subscription);
                            });
                }
            }

            return result;
        }

        public IList<Subscription> List(string messageName)
        {
            lock (_changeLock)
            {
                List<Subscription> result = new List<Subscription>();
                if (_messageTypeSubscriptions.ContainsKey(messageName))
                {
                    _messageTypeSubscriptions[messageName].ForEach(entry => result.Add(entry.Subscription));
                }

                return result;
            }
        }

        public IList<Subscription> List(string messageName, string correlationId)
        {
            List<Subscription> result = new List<Subscription>();
            lock (_changeLock)
            {
                if (_messageTypeSubscriptions.ContainsKey(messageName))
                {
                    _messageTypeSubscriptions[messageName].ForEach(
                        delegate(SubscriptionCacheEntry entry)
                            {
                                if (!result.Contains(entry.Subscription))
                                    result.Add(entry.Subscription);
                            });
                }

                string key = GetSubscriptionKey(messageName, correlationId);

                if (_correlatedSubscriptions.ContainsKey(key))
                {
                    _correlatedSubscriptions[key].ForEach(
                        delegate(SubscriptionCacheEntry entry)
                            {
                                if (!result.Contains(entry.Subscription))
                                    result.Add(entry.Subscription);
                            });
                }
            }
            return result;
        }

        public void Add(Subscription subscription)
        {
            bool added;
            lock (_changeLock)
            {
                if (string.IsNullOrEmpty(subscription.CorrelationId))
                    added = Add(_messageTypeSubscriptions, subscription);
                else
                    added = Add(_correlatedSubscriptions, subscription);
            }

            if (added)
            {
                OnAddSubscription(this, new SubscriptionEventArgs(subscription));
            }
        }

        public void Remove(Subscription subscription)
        {
            bool removed;
            lock (_changeLock)
            {
                if (string.IsNullOrEmpty(subscription.CorrelationId))
                    removed = Remove(_messageTypeSubscriptions, subscription);
                else
                    removed = Remove(_correlatedSubscriptions, subscription);
            }

            if (removed)
            {
                OnRemoveSubscription(this, new SubscriptionEventArgs(subscription));
            }
        }

        public event EventHandler<SubscriptionEventArgs> OnAddSubscription = delegate { };

        public event EventHandler<SubscriptionEventArgs> OnRemoveSubscription = delegate { };

        public void Dispose()
        {
            _messageTypeSubscriptions.Clear();
            _correlatedSubscriptions.Clear();
        }

        private static string GetSubscriptionKey(Subscription subscription)
        {
            if (string.IsNullOrEmpty(subscription.CorrelationId))
            
                return subscription.MessageName;
            return subscription.MessageName + "/" + subscription.CorrelationId;
        }

        private static string GetSubscriptionKey(string messageName, string correlationId)
        {
            if (string.IsNullOrEmpty(correlationId))
                return messageName;

            return messageName + "/" + correlationId;
        }

        private static bool Add(IDictionary<string, List<SubscriptionCacheEntry>> cache, Subscription subscription)
        {
            string key = GetSubscriptionKey(subscription);

            if (!cache.ContainsKey(key))
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Adding new local subscription list for message {0}",
                                     key);

                cache.Add(key, new List<SubscriptionCacheEntry>());
            }

            SubscriptionCacheEntry entry = new SubscriptionCacheEntry(subscription);

            if (!cache[key].Contains(entry))
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Adding new local subscription for {0} going to {1}",
                                     key,
                                     subscription.EndpointUri);

                cache[key].Add(entry);

                return true;
            }

            return false;
        }

        private static bool Remove(IDictionary<string, List<SubscriptionCacheEntry>> cache, Subscription subscription)
        {
            bool removed = false;

            string key = GetSubscriptionKey(subscription);

            if (cache.ContainsKey(key))
            {
                SubscriptionCacheEntry entry = new SubscriptionCacheEntry(subscription);

                if (cache[key].Contains(entry))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Removing local subscription for {0} going to {1}",
                                         key,
                                         subscription.EndpointUri);

                    cache[key].Remove(entry);
                }

                if (cache[key].Count == 0)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Removing local subscription list for message {0}", key);

                    cache.Remove(key);

                    removed = true;
                }
            }

            return removed;
        }
    }
}