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
	using System.Linq;
	using Stact;

	public class SubscriptionCoordinatorBusService :
		IBusService
	{
		readonly string _network;
		readonly IEnumerable<Func<IServiceBus, BusSubscriptionEventObserver>> _observers;
		IServiceBus _bus;
		//BusSubscriptionConnector _busConnector;
		IServiceBus _controlBus;
		Uri _controlUri;
		Uri _dataUri;

		bool _disposed;
		//BusSubscriptionMessageProducer _producer;
		BusSubscriptionEventListener _subscriptionEventListener;
		UnsubscribeAction _unregisterAction;
		UnsubscribeAction _unsubscribeAction;

		public SubscriptionCoordinatorBusService(string network, IEnumerable<Func<IServiceBus, BusSubscriptionEventObserver>> observers)
		{
			_network = network;
			_observers = observers;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_dataUri = bus.Endpoint.Address.Uri;
			_controlUri = bus.ControlBus.Endpoint.Address.Uri;


			var stub = new ChannelAdapter();


			// TODO Remote and Local Ignore
			// for remote endpoints, we need to pipe the remote endpoint subscriptions into a separate cache for that endpoint
			// so that they bind the the outbound pipeline
			// for the remote endpoint that matches the local bus endpoint, we need to setup a default connector
			// that does nothing so that local endpoint subscriptions from the subscription queue
			// are essentially ignored

			//_producer = new BusSubscriptionMessageProducer(_clientId, stub);

			//_busConnector = new BusSubscriptionConnector(bus);

			var observers = _observers.Select(x => x(bus)).ToArray();

			_subscriptionEventListener = new BusSubscriptionEventListener(bus, observers);

			_unregisterAction = _bus.Configure(x =>
				{
					UnregisterAction unregisterAction = x.Register(_subscriptionEventListener);

					return () => unregisterAction();
				});


			var consumerInstance = new SubscriptionMessageConsumer(stub, _network);

			_unsubscribeAction = _bus.ControlBus.SubscribeInstance(consumerInstance);
		}

		public void Stop()
		{
			_unsubscribeAction();
			_unregisterAction();
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		~SubscriptionCoordinatorBusService()
		{
			Dispose(false);
		}
	}
}