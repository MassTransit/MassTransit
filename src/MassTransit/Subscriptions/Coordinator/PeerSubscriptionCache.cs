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
	using Magnum.Extensions;
	using Messages;
	using Stact;
	using log4net;

	public class PeerSubscriptionCache
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (PeerSubscriptionCache));

		readonly IDictionary<Uri, EndpointSubscriptionCache> _endpoints;
		readonly Fiber _fiber;
		readonly SubscriptionObserver _observer;
		readonly Scheduler _scheduler;

		public PeerSubscriptionCache(Fiber fiber, Scheduler scheduler, SubscriptionObserver observer)
		{
			_observer = observer;
			_fiber = fiber;
			_scheduler = scheduler;
			_endpoints = new Dictionary<Uri, EndpointSubscriptionCache>();
		}

		public void Send(AddPeerSubscription message)
		{
			EndpointSubscriptionCache subscription;
			if (!_endpoints.TryGetValue(message.EndpointUri, out subscription))
			{
				subscription = new EndpointSubscriptionCache(_fiber, _scheduler, _observer);
				_endpoints.Add(message.EndpointUri, subscription);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("AddPeerSubscription: {0}, {1} {2}", message.MessageName, message.EndpointUri,
					message.SubscriptionId);

			subscription.Send(message);
		}

		public void Send(RemovePeerSubscription message)
		{
			EndpointSubscriptionCache subscription;
			if (_endpoints.TryGetValue(message.EndpointUri, out subscription))
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("RemovePeerSubscription: {0}, {1} {2}", message.MessageName, message.EndpointUri,
						message.SubscriptionId);

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
			_endpoints.Values.Each(x => x.Send(message));
		}

		public void Send(RemovePeer message)
		{
			_endpoints.Values.Each(x => x.Send(message));
		}
	}
}