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
	using Pipeline;

	public class SubscriptionPublisher :
		ISubscriptionEvent,
		IBusService
	{
		private IServiceBus _bus;
		private ISubscriptionService _service;
		private UnregisterAction _unregisterAction;

		/// <summary>
		/// Publishes subscription events to the ISubscriptionService
		/// </summary>
		/// <param name="service">The service that is handling the event subscriptions</param>
		public SubscriptionPublisher(ISubscriptionService service)
		{
			_service = service;
		}

		public void Dispose()
		{
			_bus = null;
			_service = null;
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_unregisterAction = _bus.InboundPipeline.Configure(x => x.Register(this));
		}

		public void Stop()
		{
			_unregisterAction();
		}

		public UnsubscribeAction SubscribedTo<T>()
			where T : class
		{
			return _service.SubscribedTo<T>(_bus.Endpoint.Uri);
		}

		public UnsubscribeAction SubscribedTo<T, K>(K correlationId)
			where T : class, CorrelatedBy<K>
		{
			return _service.SubscribedTo<T, K>(correlationId, _bus.Endpoint.Uri);
		}
	}
}