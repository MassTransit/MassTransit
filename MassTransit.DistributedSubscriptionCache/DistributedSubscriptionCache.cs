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
namespace MassTransit.DistributedSubscriptionCache
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Enyim.Caching;
	using Enyim.Caching.Memcached;
	using log4net;
	using MassTransit.ServiceBus.Subscriptions;
	using ServiceBus;

	public class DistributedSubscriptionCache :
		ISubscriptionCache
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (DistributedSubscriptionCache));

		private readonly List<string> _messageTypes = new List<string>();
		private readonly char _newLineToken = '\n';
		private readonly char _valueSeparator = '\t';

		public IList<Subscription> List()
		{
			List<Subscription> result = new List<Subscription>();

			foreach (string messageType in _messageTypes)
			{
				result.AddRange(List(messageType));
			}

			return result;
		}

		public IList<Subscription> List(string messageName)
		{
			List<Subscription> result = new List<Subscription>();

			SubscriptionKey key = new SubscriptionKey(messageName);

			QueryCache(messageName, key, result);

			return result;
		}

		public IList<Subscription> List(string messageName, string correlationId)
		{
			List<Subscription> result = new List<Subscription>();

			SubscriptionKey key = new SubscriptionKey(messageName, correlationId);

			QueryCache(messageName, key, result);

			key = new SubscriptionKey(messageName);

			QueryCache(messageName, key, result);

			return result;
		}

		public void Add(Subscription subscription)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Adding Distributed Subscription {0} : {1}", subscription.MessageName, subscription.EndpointUri);

			if (_messageTypes.Contains(subscription.MessageName) == false)
				_messageTypes.Add(subscription.MessageName);

			SubscriptionKey key = new SubscriptionKey(subscription.MessageName, subscription.CorrelationId);

			using (CacheLock cache = new CacheLock(key))
			{
				string currentValue = cache.GetFromClient<string>(key);

				string addUri = subscription.EndpointUri.ToString();

				if (currentValue == null)
				{
					cache.StoreInClient(StoreMode.Set, key, addUri, TimeSpan.FromDays(14));
					OnAddSubscription(this, new SubscriptionEventArgs(subscription));
				}
				else
				{
					if (currentValue.Contains(addUri))
					{
						cache.StoreInClient(StoreMode.Set, key, currentValue, TimeSpan.FromDays(14));
					}
					else
					{
						string newValue = currentValue + _newLineToken + addUri;
						cache.StoreInClient(StoreMode.Set, key, newValue, TimeSpan.FromDays(14));
						OnAddSubscription(this, new SubscriptionEventArgs(subscription));
					}
				}
			}
		}

		public void Remove(Subscription subscription)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Removing Distributed Subscription {0} : {1}", subscription.MessageName, subscription.EndpointUri);

			SubscriptionKey key = new SubscriptionKey(subscription.MessageName, subscription.CorrelationId);

			using (CacheLock cache = new CacheLock(key))
			{
				string currentValue = cache.GetFromClient<string>(key);

				string removeUri = subscription.EndpointUri.ToString();

				if (!string.IsNullOrEmpty(currentValue))
				{
					if (currentValue.Contains(removeUri))
					{
						int start = currentValue.IndexOf(removeUri);
						if (start > 0) //Modify the start to include the '\n'
						{
							start--;
						}

						string newValue = currentValue.Remove(start, removeUri.Length + 1);
						cache.StoreInClient(StoreMode.Set, key, newValue, TimeSpan.FromDays(14));
						OnRemoveSubscription(this, new SubscriptionEventArgs(subscription));
					}
				}
			}
		}

		public event EventHandler<SubscriptionEventArgs> OnAddSubscription = delegate { };

		public event EventHandler<SubscriptionEventArgs> OnRemoveSubscription = delegate { };

		public void Dispose()
		{
			_messageTypes.Clear();
		}

		private void QueryCache(string messageName, SubscriptionKey key, List<Subscription> result)
		{
			MemcachedClient client = new MemcachedClient();

			string value = client.Get<string>(key.CacheKey);

			if (!string.IsNullOrEmpty(value))
			{
				string[] uris = value.Split(_newLineToken);

				foreach (string uri in uris)
				{
					result.Add(new Subscription(messageName, key.CorrelationId, new Uri(uri)));
				}
			}
		}

		internal class CacheLock : IDisposable
		{
			private const int _lockTimeSpan = 5;
			private readonly MemcachedClient _client = new MemcachedClient();
			private readonly SubscriptionKey _key;

			public CacheLock(SubscriptionKey key)
			{
				_key = key;

				int retryCount = 0;
				while (_client.Store(StoreMode.Add, key.LockKey, true, TimeSpan.FromSeconds(_lockTimeSpan)) == false)
				{
					retryCount++;
					if (retryCount == 50)
						throw new ApplicationException("Unable to store the subscription information");

					Thread.Sleep(20);
				}
			}

			public MemcachedClient Client
			{
				get { return _client; }
			}

			public void Dispose()
			{
				_client.Remove(_key.LockKey);
			}

			public R GetFromClient<R>(SubscriptionKey key)
			{
				return _client.Get<R>(key.CacheKey);
			}

			public void StoreInClient(StoreMode mode, SubscriptionKey key, object value, TimeSpan validFor)
			{
				_client.Store(mode, key.CacheKey, value, validFor);
			}
		}

		internal class SubscriptionKey
		{
			private readonly string _baseKey;
			private readonly string _correlationId;
			private readonly string _messageName;

			public SubscriptionKey(string messageName)
			{
				_messageName = messageName;
				_baseKey = messageName;
			}

			public SubscriptionKey(string messageName, string correlationId)
				: this(messageName)
			{
				_correlationId = correlationId;
				if (!string.IsNullOrEmpty(correlationId))
					_baseKey += "/" + correlationId;
			}

			public string MessageName
			{
				get { return _messageName; }
			}

			public virtual string CacheKey
			{
				get { return string.Format("/mt/{0}", _baseKey); }
			}

			public virtual string LockKey
			{
				get { return string.Format("/lock/mt/{0}", _baseKey); }
			}

			public string CorrelationId
			{
				get { return _correlationId; }
			}

			public override string ToString()
			{
				return string.Format("SubscriptionKey for {0}", _baseKey);
			}
		}
	}
}