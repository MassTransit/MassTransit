using System;
using System.Collections.Generic;

namespace MassTransit.ServiceBus.Subscriptions
{
    public class SubscriptionCache : //this is effectively the inmemory stuff right?
        ISubscriptionStorage
    {
        private readonly Dictionary<Type, List<SubscriptionCacheEntry>> _messageTypeSubscriptions =
            new Dictionary<Type, List<SubscriptionCacheEntry>>();

        #region ISubscriptionStorage Members

        public IList<IEndpoint> List<T>(params T[] messages)
        {
            List<IEndpoint> result = new List<IEndpoint>();
            if (_messageTypeSubscriptions.ContainsKey(typeof (T)))
            {
                _messageTypeSubscriptions[typeof (T)].ForEach(
                    delegate(SubscriptionCacheEntry entry) { result.Add(entry.Endpoint); });
            }

            return result;
        }

        public void Add(Type messageType, IEndpoint endpoint)
        {
            if (!_messageTypeSubscriptions.ContainsKey(messageType))
                _messageTypeSubscriptions.Add(messageType, new List<SubscriptionCacheEntry>());

            SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

            if (!_messageTypeSubscriptions[messageType].Contains(entry))
                _messageTypeSubscriptions[messageType].Add(entry);
        }

        public void Remove(Type messageType, IEndpoint endpoint)
        {
            if (_messageTypeSubscriptions.ContainsKey(messageType))
            {
                SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

                if (_messageTypeSubscriptions[messageType].Contains(entry))
                    _messageTypeSubscriptions[messageType].Remove(entry);

                if (_messageTypeSubscriptions[messageType].Count == 0)
                    _messageTypeSubscriptions.Remove(messageType);
            }
        }

        #endregion
    }
}