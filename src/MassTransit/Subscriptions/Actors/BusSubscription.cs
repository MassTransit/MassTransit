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
namespace MassTransit.Subscriptions.Actors
{
	using System;
	using System.Collections.Generic;
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using log4net;

	public class BusSubscription
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (BusSubscription));
		readonly string _messageName;
		readonly IEnumerable<BusSubscriptionEventObserver> _observers;
		Uri _endpointUri;
		HashSet<Guid> _ids;
		Guid _subscriptionId;

		public BusSubscription(string messageName, IEnumerable<BusSubscriptionEventObserver> observers)
		{
			_messageName = messageName;
			_observers = observers;

			_ids = new HashSet<Guid>();

			_subscriptionId = Guid.Empty;
		}

		public void OnSubscribeTo(SubscribeTo added)
		{
			bool wasAdded = _ids.Add(added.SubscriptionId);

			if (!wasAdded || _ids.Count != 1) 
				return;

			_subscriptionId = CombGuid.Generate();
			_endpointUri = added.EndpointUri;

			var add = new SubscriptionAddedMessage
				{
					SubscriptionId = _subscriptionId,
					EndpointUri = _endpointUri,
					MessageName = _messageName
				};

			_log.DebugFormat("SubscribeTo: {0}, {1}", _messageName, _subscriptionId);

			_observers.Each(x => x.OnSubscriptionAdded(add));
		}

		public void OnUnsubscribeFrom(UnsubscribeFrom removed)
		{
			if (!_ids.Contains(removed.SubscriptionId)) 
				return;
			
			_ids.Clear();

			var remove = new SubscriptionRemovedMessage
				{
					SubscriptionId = _subscriptionId,
					EndpointUri = _endpointUri,
					MessageName = _messageName
				};

			_log.DebugFormat("UnsubscribeFrom: {0}, {1}", _messageName, _subscriptionId);

			_observers.Each(x => x.OnSubscriptionRemoved(remove));
			_subscriptionId = Guid.Empty;
		}
	}
}