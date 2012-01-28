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

using MassTransit.Util;

namespace MassTransit.Services.Subscriptions
{
	using System;
	using Pipeline;

	/// <summary>
	/// Used by testing framework
	/// </summary>
	public class SubscriptionConsumer :
		IEndpointSubscriptionEvent,
		IBusService
	{
		readonly ISubscriptionService _service;
		IServiceBus _bus;
		IOutboundMessagePipeline _pipeline;
		UnregisterAction _unregisterAction;

		public SubscriptionConsumer(ISubscriptionService service)
		{
			_service = service;
		}

		public void Dispose()
		{
		}

		public void Start(IServiceBus bus)
		{
			if (bus == null) throw new ArgumentNullException("bus");
			_bus = bus;
			_pipeline = _bus.OutboundPipeline;
			_unregisterAction = _service.Register(this);
		}

		public void Stop()
		{
			_unregisterAction();
		}

		public UnsubscribeAction SubscribedTo<TMessage>(Uri endpointUri)
			where TMessage : class
		{
			// messages to the same bus that we're on should not be connected further
			if (endpointUri == _bus.Endpoint.Address.Uri)
				return () => true;

			// other messages should be connected to the passed endpoint uri
			IEndpoint endpoint = _bus.GetEndpoint(endpointUri);

			return _pipeline.ConnectEndpoint<TMessage>(endpoint);
		}

		public UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId, Uri endpointUri)
			where TMessage : class, CorrelatedBy<TKey>
		{
			if (endpointUri == _bus.Endpoint.Address.Uri)
				return () => true;

			IEndpoint endpoint = _bus.GetEndpoint(endpointUri);

			return _pipeline.ConnectEndpoint<TMessage, TKey>(correlationId, endpoint);
		}
	}
}