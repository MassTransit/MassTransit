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
	using Messages;
	using log4net;

	public class BusSubscriptionCache
	{
		readonly BusSubscriptionEventObserver _observer;
		static readonly ILog _log = LogManager.GetLogger(typeof (BusSubscriptionCache));

		readonly IDictionary<string, BusSubscription> _typeActors;

		public BusSubscriptionCache(BusSubscriptionEventObserver observer)
		{
			_observer = observer;
			_typeActors = new Dictionary<string, BusSubscription>();
		}

		public void OnSubscribeTo(SubscribeTo message)
		{
			BusSubscription busSubscription;
			if (!_typeActors.TryGetValue(message.MessageName, out busSubscription))
			{
				busSubscription = new BusSubscription(message.MessageName, _observer);
				_typeActors.Add(message.MessageName, busSubscription);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("SubscribeTo: {0}, {1}", message.MessageName, message.SubscriptionId);

			busSubscription.OnSubscribeTo(message);
		}

		public void OnUnsubscribeFrom(UnsubscribeFrom message)
		{
			BusSubscription actor;
			if (_typeActors.TryGetValue(message.MessageName, out actor))
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("UnsubscribeFrom: {0}, {1}", message.MessageName, message.SubscriptionId);

				actor.OnUnsubscribeFrom(message);
			}
			else
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("UnsubscribeFrom(unknown): {0}, {1}", message.MessageName, message.SubscriptionId);
			}
		}
	}
}