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
	using log4net;
	using Actor = Stact.Actor;

	public class OutboundPipelineSubscriptionConnector :
		Actor
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (OutboundPipelineSubscriptionConnector));
		readonly IServiceBus _bus;
		readonly EndpointConnectorCache _cache;
		readonly Dictionary<Guid, UnsubscribeAction> _connectionCache;

		public OutboundPipelineSubscriptionConnector(IServiceBus bus)
		{
			_bus = bus;
			_cache = new EndpointConnectorCache(bus);
			_connectionCache = new Dictionary<Guid, UnsubscribeAction>();
		}

		public void Handle(Stact.Message<SubscriptionAdded> message)
		{
			_connectionCache[message.Body.SubscriptionId] = _cache.Connect(message.Body.MessageName, _bus.Endpoint.Address.Uri,
				message.Body.CorrelationId);

			if (_log.IsDebugEnabled)
				_log.DebugFormat("SubscriptionAdded: {0}, {1}", message.Body.MessageName, message.Body.SubscriptionId);
		}

		public void Handle(Stact.Message<SubscriptionRemoved> message)
		{
			UnsubscribeAction unsubscribe;
			if (_connectionCache.TryGetValue(message.Body.SubscriptionId, out unsubscribe))
			{
				unsubscribe();
				_connectionCache.Remove(message.Body.SubscriptionId);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("SubscriptionRemoved: {0}, {1}", message.Body.MessageName, message.Body.SubscriptionId);
		}
	}
}