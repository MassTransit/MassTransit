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
	using Pipeline;

	public class PublishEndpointInterceptor :
		IMessageInterceptor
	{
		readonly HashSet<Type> _added;
		readonly IServiceBus _bus;
		readonly IEndpointAddressProvider _provider;

		public PublishEndpointInterceptor(IServiceBus bus, IEndpointAddressProvider provider)
		{
			_bus = bus;
			_provider = provider;

			_added = new HashSet<Type>();
		}

		public void PreDispatch(object message)
		{
			lock (_added)
			{
				Type messageType = message.GetType();

				if (_added.Contains(messageType))
					return;

				AddEndpointForType(messageType);
			}
		}

		public void PostDispatch(object message)
		{
		}

		void AddEndpointForType(Type messageType)
		{
			IEnumerable<Uri> addresses = _provider.GetAddressForMessage(messageType);

			foreach (Uri address in addresses)
			{
				FindOrAddEndpoint(messageType, address);
			}
		}

		void FindOrAddEndpoint(Type messageType, Uri address)
		{
			var locator = new PublishEndpointSinkLocator(messageType, address);
			_bus.OutboundPipeline.Inspect(locator);

			if (locator.Found)
			{
				_added.Add(messageType);
				return;
			}

			IEndpoint endpoint = _bus.GetEndpoint(address);

			_bus.OutboundPipeline.ConnectEndpoint(messageType, endpoint);
		}
	}
}