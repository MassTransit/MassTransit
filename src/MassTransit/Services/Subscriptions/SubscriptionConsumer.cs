// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Services.Subscriptions
{
	using System;
	using Internal;
	using Pipeline;

	public class SubscriptionConsumer :
		IEndpointSubscriptionEvent,
		IBusService
	{
		private IServiceBus _bus;
		private IEndpointResolver _endpointResolver;
		private IMessagePipeline _pipeline;
		private ISubscriptionService _service;
		private UnregisterAction _unregisterAction;

		public SubscriptionConsumer(ISubscriptionService service, IEndpointResolver endpointResolver)
		{
			_service = service;
			_endpointResolver = endpointResolver;
		}

		public void Dispose()
		{
			_pipeline = null;
			_bus = null;
			_service = null;
			_endpointResolver = null;
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_pipeline = _bus.OutboundPipeline;
			_unregisterAction = _service.Register(this);
		}

		public void Stop()
		{
			_unregisterAction();
		}

		public UnsubscribeAction SubscribedTo<T>(Uri endpointUri)
			where T : class
		{
			if (endpointUri == _bus.Endpoint.Uri)
				return () => true;

			IEndpoint endpoint = _endpointResolver.GetEndpoint(endpointUri);

			return _pipeline.Subscribe<T>(endpoint);
		}

		public UnsubscribeAction SubscribedTo<T, K>(K correlationId, Uri endpointUri)
			where T : class, CorrelatedBy<K>
		{
			if (endpointUri == _bus.Endpoint.Uri)
				return () => true;

			IEndpoint endpoint = _endpointResolver.GetEndpoint(endpointUri);

			return _pipeline.Subscribe<T, K>(correlationId, endpoint);
		}
	}
}