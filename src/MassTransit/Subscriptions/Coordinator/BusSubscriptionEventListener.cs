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
	using System;
	using System.Collections.Generic;
	using Magnum;
	using Messages;
	using Pipeline;
	using log4net;

	public class BusSubscriptionEventListener :
		ISubscriptionEvent
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (BusSubscriptionEventListener));
		readonly BusSubscriptionCache _busSubscriptionCache;
		readonly Uri _endpointUri;

		public BusSubscriptionEventListener(IServiceBus bus, SubscriptionObserver observer)
		{
			_endpointUri = bus.Endpoint.Address.Uri;

			_busSubscriptionCache = new BusSubscriptionCache(observer);
		}

		public IEnumerable<Subscription> Subscriptions
		{
			get { return _busSubscriptionCache.Subscriptions; }
		}

		public UnsubscribeAction SubscribedTo<TMessage>()
			where TMessage : class
		{
			return Subscribe<TMessage>(null);
		}

		public UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId)
			where TMessage : class, CorrelatedBy<TKey>
		{
			return Subscribe<TMessage>(string.Format("{0}", correlationId));
		}

		UnsubscribeAction Subscribe<TMessage>(string correlationId)
			where TMessage : class
		{
			Guid subscriptionId = CombGuid.Generate();
			string messageName = typeof (TMessage).ToMessageName();

			var subscribeTo = new SubscribeToMessage
				{
					SubscriptionId = subscriptionId,
					EndpointUri = _endpointUri,
					MessageName = messageName,
					CorrelationId = correlationId,
				};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("SubscribeTo: {0}, {1}", subscribeTo.MessageName, subscribeTo.SubscriptionId);

			_busSubscriptionCache.OnSubscribeTo(subscribeTo);

			return () => Unsubscribe(subscriptionId, messageName, correlationId);
		}

		bool Unsubscribe(Guid subscriptionId, string messageName, string correlationId)
		{
			var unsubscribeFrom = new UnsubscribeFromMessage
				{
					SubscriptionId = subscriptionId,
					MessageName = messageName,
					CorrelationId = correlationId,
				};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("UnsubscribeFrom: {0}, {1}", unsubscribeFrom.MessageName, unsubscribeFrom.SubscriptionId);

			_busSubscriptionCache.OnUnsubscribeFrom(unsubscribeFrom);

			return true;
		}
	}
}