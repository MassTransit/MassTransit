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

namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using Messages;

    public class LocalSubscriptionCache : 
        ISubscriptionStorage
    {
        //<messageName, address>
        private readonly Dictionary<string, List<SubscriptionCacheEntry>> _messageTypeSubscriptions =
            new Dictionary<string, List<SubscriptionCacheEntry>>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly ILog _log = LogManager.GetLogger(typeof(LocalSubscriptionCache));

        // just a shared local cache
        public LocalSubscriptionCache()
        {
            
        }

        public event EventHandler<SubscriptionChangedEventArgs> SubscriptionChanged;

        public IList<Subscription> List(string messageName)
        {
            List<Subscription> result = new List<Subscription>();
            if (_messageTypeSubscriptions.ContainsKey(messageName))
            {
                _messageTypeSubscriptions[messageName].ForEach(
                    delegate(SubscriptionCacheEntry entry) { result.Add(new Subscription(entry.Endpoint, messageName)); });
            }

            return result;
        }


        public IList<Subscription> List()
        {
            List<Subscription> result = new List<Subscription>();

            foreach (KeyValuePair<string, List<SubscriptionCacheEntry>> pair in _messageTypeSubscriptions)
            {
                pair.Value.ForEach(delegate(SubscriptionCacheEntry e) { result.Add(new Subscription(e.Endpoint, pair.Key)); });
            }

            return result;
        }

        public void Add(string messageName, Uri endpoint)
        {
            InternalAdd(messageName, endpoint);
        }
        public void Remove(string messageName, Uri endpoint)
        {
            InternalRemove(messageName, endpoint);			
        }

        public void Dispose()
        {
            _messageTypeSubscriptions.Clear();
        }

        
        private void InternalRemove(string messageName, Uri endpoint)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Removing Local Subscription {0} : {1}", messageName, endpoint);

            lock (this)
            {
                if (_messageTypeSubscriptions.ContainsKey(messageName))
                {
                    SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

                    if (_messageTypeSubscriptions[messageName].Contains(entry))
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Removing local subscription entry for endpoint {0} on {1}", endpoint,
                                             GetHashCode());
                        _messageTypeSubscriptions[messageName].Remove(entry);
                    }

                    if (_messageTypeSubscriptions[messageName].Count == 0)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Removing local subscription list for type {0} on {1}", messageName, GetHashCode());
                        _messageTypeSubscriptions.Remove(messageName);

                        OnChange(new SubscriptionChange(messageName, endpoint, SubscriptionChangeType.Remove));
                    }
                }   
            }
        }
        private void InternalAdd(string messageName, Uri endpoint)
        {
            lock (this)
            {
                if (!_messageTypeSubscriptions.ContainsKey(messageName))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Adding new local subscription list for type {0} on {1}", messageName, GetHashCode());
                    _messageTypeSubscriptions.Add(messageName, new List<SubscriptionCacheEntry>());
                }

                SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

                if (!_messageTypeSubscriptions[messageName].Contains(entry))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Adding new local subscription entry for endpoint {0} on {1}", endpoint,
                                         GetHashCode());
                    _messageTypeSubscriptions[messageName].Add(entry);

                    OnChange(new SubscriptionChange(messageName, endpoint, SubscriptionChangeType.Add));
                }
            }
        }

        protected void OnChange(SubscriptionChange change)
        {
            EventHandler<SubscriptionChangedEventArgs> handler = this.SubscriptionChanged;
            if (handler != null)
            {
                handler(this, new SubscriptionChangedEventArgs(change));
            }
        }
    }
}