using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace MassTransit.ServiceBus.Subscriptions
{
    public class SubscriptionCache : //this is effectively the inmemory stuff right?
        ISubscriptionStorage
    {
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		
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
			{
				_log.DebugFormat("Adding new subscription list for type {0} on {1}", messageType.ToString(), GetHashCode());
				_messageTypeSubscriptions.Add(messageType, new List<SubscriptionCacheEntry>());
			}

        	SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

			if (!_messageTypeSubscriptions[messageType].Contains(entry))
			{
				_log.DebugFormat("Adding new subscription entry for endpoint {0} on {1}", endpoint.Transport.Address, GetHashCode());
				_messageTypeSubscriptions[messageType].Add(entry);
			}
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