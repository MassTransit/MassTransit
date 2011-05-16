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
	using Context;
	using Exceptions;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Management;
	using Pipeline.Sinks;
	using Util;

	public class PublishEndpointInterceptor :
		IOutboundMessageInterceptor
	{
		readonly IDictionary<Type, UnsubscribeAction> _added;
		readonly IServiceBus _bus;
		readonly InboundRabbitMqTransport _inboundTransport;
		IRabbitMqEndpointAddress _address;

		public PublishEndpointInterceptor(IServiceBus bus)
		{
			_bus = bus;

			_inboundTransport = _bus.Endpoint.InboundTransport as InboundRabbitMqTransport;
			if (_inboundTransport == null)
				throw new ConfigurationException("The bus must be receiving from a RabbitMQ endpoint for this interceptor to work");

			_address = _inboundTransport.Address.CastAs<IRabbitMqEndpointAddress>();

			_added = new Dictionary<Type, UnsubscribeAction>();
		}

		public void PreDispatch(ISendContext context)
		{
			lock (_added)
			{
				Type messageType = context.DeclaringMessageType;

				if (_added.ContainsKey(messageType))
					return;

				AddEndpointForType(messageType);
			}
		}

		public void PostDispatch(ISendContext context)
		{
		}

		void AddEndpointForType(Type messageType)
		{
			using (var management = new RabbitMqEndpointManagement(_address))
			{
				IEnumerable<Type> types = management.BindExchangesForPublisher(messageType);
				foreach (Type type in types)
				{
					if (_added.ContainsKey(type))
						continue;

					var messageName = new MessageName(type);

					IRabbitMqEndpointAddress messageEndpointAddress = _address.ForQueue(messageName.ToString());

					FindOrAddEndpoint(type, messageEndpointAddress);
				}
			}
		}

		void FindOrAddEndpoint(Type messageType, IRabbitMqEndpointAddress address)
		{
			var locator = new PublishEndpointSinkLocator(messageType, address);
			_bus.OutboundPipeline.Inspect(locator);

			if (locator.Found)
			{
				_added.Add(messageType, () => true);
				return;
			}

			IEndpoint endpoint = _bus.GetEndpoint(address.Uri);

			this.FastInvoke(new[] {messageType}, "CreateEndpointSink", endpoint);
		}

		[UsedImplicitly]
		void CreateEndpointSink<TMessage>(IEndpoint endpoint)
			where TMessage : class
		{
			var endpointSink = new EndpointMessageSink<TMessage>(endpoint);

			//var filterSink = new InboundMessageFilter<TMessage>("Type-specific", endpointSink,
			//	message => message.GetType() == typeof (TMessage));

			UnsubscribeAction unsubscribeAction = _bus.OutboundPipeline.ConnectToRouter(endpointSink);

			_added.Add(typeof (TMessage), unsubscribeAction);
		}
	}
}