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
namespace MassTransit.Transports.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using Logging;
	using Magnum;
	using Magnum.Extensions;
	using Management;
	using RabbitMQ.Client;
	using Subscriptions.Coordinator;
	using Subscriptions.Messages;

    public class RabbitMqSubscriptionBinder :
		SubscriptionObserver
	{
		static readonly ILog _log = Logger.Get(typeof (RabbitMqSubscriptionBinder));
		readonly Dictionary<Guid, MessageName> _bindings;
		IRabbitMqEndpointAddress _inputAddress;

		public RabbitMqSubscriptionBinder(IServiceBus bus)
		{
			_bindings = new Dictionary<Guid, MessageName>();

			_inputAddress = bus.Endpoint.InboundTransport.Address.CastAs<IRabbitMqEndpointAddress>();
		}

		public void OnSubscriptionAdded(SubscriptionAdded message)
		{
			Guard.AgainstNull(_inputAddress, "InputAddress", "The input address was not set");

			Type messageType = Type.GetType(message.MessageName);
			if (messageType == null)
			{
				_log.InfoFormat("Unknown message type '{0}', unable to add subscription", message.MessageName);
				return;
			}

			var messageName = new MessageName(messageType);

			using (var management = new RabbitMqEndpointManagement(_inputAddress))
			{
				management.BindExchange(_inputAddress.Name, messageName.ToString(), ExchangeType.Fanout, "");
			}

			_bindings[message.SubscriptionId] = messageName;
		}

		public void OnSubscriptionRemoved(SubscriptionRemoved message)
		{
			Guard.AgainstNull(_inputAddress, "InputAddress", "The input address was not set");

			MessageName messageName;
			if (_bindings.TryGetValue(message.SubscriptionId, out messageName))
			{
				using (var management = new RabbitMqEndpointManagement(_inputAddress))
				{
					management.UnbindExchange(_inputAddress.Name, messageName.ToString(), "");
				}

				_bindings.Remove(message.SubscriptionId);
			}
		}

		public void OnComplete()
		{
		}
	}
}