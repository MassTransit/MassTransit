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
	using Magnum.Extensions;
	using Management;
	using Pipeline;
	using RabbitMQ.Client;

	public class RabbitMqSubscriptionBinder :
		ISubscriptionEvent
	{
		readonly IRabbitMqEndpointAddress _inputAddress;

		public RabbitMqSubscriptionBinder(IInboundTransport inboundTransport)
		{
			_inputAddress = inboundTransport.Address.CastAs<IRabbitMqEndpointAddress>();
		}

		public UnsubscribeAction SubscribedTo<T>()
			where T : class
		{
			var messageName = new MessageName(typeof (T));

			string routingKey = "";

			return BindQueue(messageName, routingKey);
		}

		public UnsubscribeAction SubscribedTo<T, K>(K correlationId)
			where T : class, CorrelatedBy<K>
		{
			var messageName = new MessageName(typeof (T));

			string routingKey = correlationId.ToString();

			return BindQueue(messageName, routingKey);
		}

		UnsubscribeAction BindQueue(MessageName messageName, string routingKey)
		{
			using (var management = new RabbitMqEndpointManagement(_inputAddress))
			{
				management.BindExchange(_inputAddress.Name, messageName.ToString(), ExchangeType.Fanout, routingKey);
			}

			return () =>
				{
					using (var management = new RabbitMqEndpointManagement(_inputAddress))
					{
						management.UnbindExchange(_inputAddress.Name, messageName.ToString(), routingKey);
					}

					return true;
				};
		}
	}
}