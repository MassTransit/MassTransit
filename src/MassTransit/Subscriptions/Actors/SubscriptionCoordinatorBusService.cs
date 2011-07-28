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
	using Magnum;
	using Magnum.Extensions;
	using Stact;

	public class SubscriptionCoordinatorBusService :
		IBusService
	{
		IServiceBus _bus;
		Guid _clientId;
		IServiceBus _controlBus;
		Uri _controlUri;
		Uri _dataUri;

		bool _disposed;
		ActorInstance _producer;
		BusSubscriptionEventListener _subscriptionEventListener;
		UnsubscribeAction _unregisterAction;
		UnsubscribeAction _unsubscribeAction;
		ActorInstance _busConnector;

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


			_clientId = CombGuid.Generate();

			ActorFactory<EndpointSubscriptionMessageProducer> factory = ActorFactory.Create(
				() => new EndpointSubscriptionMessageProducer(_clientId, bus.Endpoint.Address.Uri, stub));

			_producer = factory.GetActor();

			_busConnector = ActorFactory.Create<BusSubscriptionConnector>(x =>
				{
					x.ConstructedBy(() => new BusSubscriptionConnector(bus));
					x.HandleOnCallingThread();
				}).GetActor();

			var broadcast = new BroadcastChannel(new UntypedChannel[] {_busConnector, _producer});

			_subscriptionEventListener = new BusSubscriptionEventListener(broadcast);

			_unregisterAction = _bus.Configure(x =>
				{
					UnregisterAction unregisterAction = x.Register(_subscriptionEventListener);

					return () => unregisterAction();
				});


			var consumerInstance = new SubscriptionMessageConsumer(stub);

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
				if (_subscriptionEventListener != null)
				{
					_subscriptionEventListener.Dispose();
					_subscriptionEventListener = null;
				}

				if(_busConnector != null)
				{
					_busConnector.ExitOnDispose(30.Seconds()).Dispose();
					_busConnector = null;
				}

				if (_producer != null)
				{
					_producer.ExitOnDispose(30.Seconds()).Dispose();
					_producer = null;
				}
			}

			_disposed = true;
		}

		~SubscriptionCoordinatorBusService()
		{
			Dispose(false);
		}
	}
}