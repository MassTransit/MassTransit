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
namespace MassTransit.Services.Subscriptions
{
	using Pipeline;

	public class SubscriptionPublisher :
		ISubscriptionEvent,
		IBusService
	{
		readonly ISubscriptionService _service;
		IServiceBus _bus;
		UnsubscribeAction _unregisterAction;

		public SubscriptionPublisher(ISubscriptionService service)
		{
			_service = service;
		}

		public void Dispose()
		{
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_unregisterAction = _bus.Configure(x =>
				{
					UnregisterAction unregisterAction = x.Register(this);

					return () => unregisterAction();
				});
		}

		public void Stop()
		{
			_unregisterAction();
		}

		public UnsubscribeAction SubscribedTo<TMessage>()
			where TMessage : class
		{
			return _service.SubscribedTo<TMessage>(_bus.Endpoint.Address.Uri);
		}

		public UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId)
			where TMessage : class, CorrelatedBy<TKey>
		{
			return _service.SubscribedTo<TMessage, TKey>(correlationId, _bus.Endpoint.Address.Uri);
		}
	}
}