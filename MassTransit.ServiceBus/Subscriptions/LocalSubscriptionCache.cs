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

	public class LocalSubscriptionCache :
		ISubscriptionCache
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (LocalSubscriptionCache));

		private readonly Dictionary<string, List<SubscriptionCacheEntry>> _messageTypeSubscriptions =
			new Dictionary<string, List<SubscriptionCacheEntry>>(StringComparer.InvariantCultureIgnoreCase);

		private readonly object addLock = new object();
		private readonly object deleteLock = new object();

		#region ISubscriptionCache Members

		public IList<Subscription> List()
		{
			List<Subscription> result = new List<Subscription>();

			foreach (KeyValuePair<string, List<SubscriptionCacheEntry>> pair in _messageTypeSubscriptions)
			{
				pair.Value.ForEach(
					delegate(SubscriptionCacheEntry e) { result.Add(e.Subscription); });
			}

			return result;
		}

		public IList<Subscription> List(string messageName)
		{
			List<Subscription> result = new List<Subscription>();
			if (_messageTypeSubscriptions.ContainsKey(messageName))
			{
				_messageTypeSubscriptions[messageName].ForEach(
					delegate(SubscriptionCacheEntry entry) { result.Add(entry.Subscription); });
			}

			return result;
		}

		public void Add(Subscription subscription)
		{
			lock (addLock)
			{
				if (!_messageTypeSubscriptions.ContainsKey(subscription.MessageName))
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Adding new local subscription list for message {0}",
						                 subscription.MessageName);

					_messageTypeSubscriptions.Add(subscription.MessageName, new List<SubscriptionCacheEntry>());
				}

				SubscriptionCacheEntry entry = new SubscriptionCacheEntry(subscription);

				if (!_messageTypeSubscriptions[subscription.MessageName].Contains(entry))
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Adding new local subscription for {0} going to {1}",
						                 subscription.MessageName,
						                 subscription.EndpointUri);

					_messageTypeSubscriptions[subscription.MessageName].Add(entry);

					if (OnAddSubscription != null)
					{
						OnAddSubscription(this, new SubscriptionEventArgs(subscription));
					}
				}
			}
		}

		public void Remove(Subscription subscription)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Removing Local Subscription {0} : {1}", subscription.MessageName, subscription.EndpointUri);

			lock (deleteLock)
			{
				if (_messageTypeSubscriptions.ContainsKey(subscription.MessageName))
				{
					SubscriptionCacheEntry entry = new SubscriptionCacheEntry(subscription);

					if (_messageTypeSubscriptions[subscription.MessageName].Contains(entry))
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("Removing local subscription for {0} going to {1}",
							                 subscription.MessageName,
							                 subscription.EndpointUri);

						_messageTypeSubscriptions[subscription.MessageName].Remove(entry);
					}

					if (_messageTypeSubscriptions[subscription.MessageName].Count == 0)
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("Removing local subscription list for message",
							                 subscription.MessageName);

						_messageTypeSubscriptions.Remove(subscription.MessageName);

						if (OnRemoveSubscription != null)
						{
							OnRemoveSubscription(this, new SubscriptionEventArgs(subscription));
						}
					}
				}
			}
		}

		public event EventHandler<SubscriptionEventArgs> OnAddSubscription;

		public event EventHandler<SubscriptionEventArgs> OnRemoveSubscription;

		public void Dispose()
		{
			_messageTypeSubscriptions.Clear();
		}

		#endregion
	}
}