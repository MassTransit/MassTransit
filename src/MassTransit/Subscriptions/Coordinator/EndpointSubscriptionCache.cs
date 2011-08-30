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
	using Magnum.Extensions;
	using Messages;
	using Stact;
	using log4net;

	public class EndpointSubscriptionCache
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (EndpointSubscriptionCache));

		readonly IDictionary<SubscriptionKey, EndpointSubscription> _messageSubscriptions;
		readonly Fiber _fiber;
		readonly Scheduler _scheduler;
		readonly SubscriptionObserver _observer;

		public EndpointSubscriptionCache(Fiber fiber, Scheduler scheduler, SubscriptionObserver observer)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_observer = observer;
			_messageSubscriptions = new Dictionary<SubscriptionKey, EndpointSubscription>();
		}

		public void Send(AddPeerSubscription message)
		{
            var key = new SubscriptionKey
            {
                MessageName = message.MessageName,
                CorrelationId = message.CorrelationId,
            };

            EndpointSubscription subscription;
			if (!_messageSubscriptions.TryGetValue(key, out subscription))
			{
				subscription = new EndpointSubscription(_fiber, _scheduler, message.MessageName, message.CorrelationId, _observer);
				_messageSubscriptions.Add(key, subscription);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("AddPeerSubscription: {0}, {1}", message.MessageName, message.SubscriptionId);

			subscription.Send(message);
		}

		public void Send(RemovePeerSubscription message)
		{
            var key = new SubscriptionKey
            {
                MessageName = message.MessageName,
                CorrelationId = message.CorrelationId,
            };


			EndpointSubscription subscription;
			if (_messageSubscriptions.TryGetValue(key, out subscription))
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("RemovePeerSubscription: {0}, {1}", message.MessageName, message.SubscriptionId);

				subscription.Send(message);
			}
			else
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("RemovePeerSubscription(unknown): {0}, {1}", message.MessageName, message.SubscriptionId);
			}
		}

		public void Send(AddPeer message)
		{
			_messageSubscriptions.Values.Each(x => x.Send(message));
		}

		public void Send(RemovePeer message)
		{
			_messageSubscriptions.Values.Each(x => x.Send(message));
		}
	}
}