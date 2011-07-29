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
	using Magnum.Extensions;
	using Messages;
	using Stact;

	public class SubscriptionCoordinatorBusService :
		IBusService,
		BusSubscriptionCoordinator,
		BusSubscriptionEventObserver
	{
		readonly string _network;
		readonly IList<BusSubscriptionEventObserver> _observers;
		IServiceBus _bus;
		IServiceBus _controlBus;
		Uri _controlUri;
		Uri _dataUri;

		bool _disposed;
		ActorInstance _peerCache;
		UnsubscribeAction _unregister;

		public SubscriptionCoordinatorBusService(string network)
		{
			_network = network;
			_observers = new List<BusSubscriptionEventObserver>();

			_unregister = () => true;
		}

		public void AddObserver(BusSubscriptionEventObserver observer)
		{
			lock (_observers)
				_observers.Add(observer);
		}

		public void Send(AddPeerSubscription message)
		{
			if (_peerCache != null)
				_peerCache.Send(message);
		}

		public void Send(RemovePeerSubscription message)
		{
			if (_peerCache != null)
				_peerCache.Send(message);
		}

		public void Send(AddPeer message)
		{
			if (_peerCache != null)
				_peerCache.Send(message);
		}

		public void Send(RemovePeer message)
		{
			if (_peerCache != null)
				_peerCache.Send(message);
		}

		public void OnSubscriptionAdded(SubscriptionAdded message)
		{
			lock (_observers)
				_observers.Each(x => x.OnSubscriptionAdded(message));
		}

		public void OnSubscriptionRemoved(SubscriptionRemoved message)
		{
			lock (_observers)
				_observers.Each(x => x.OnSubscriptionRemoved(message));
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

			var connector = new BusSubscriptionConnector(bus);

			_peerCache = ActorFactory.Create(() => new PeerCache(connector))
				.GetActor();

			ListenToBus(bus);
		}

		public void Stop()
		{
			_unregister();
		}

		void ListenToBus(IServiceBus bus)
		{
			var subscriptionEventListener = new BusSubscriptionEventListener(bus, this);

			_unregister += _bus.Configure(x =>
				{
					UnregisterAction unregisterAction = x.Register(subscriptionEventListener);

					return () => unregisterAction();
				});

			IServiceBus controlBus = bus.ControlBus;
			if (controlBus != bus)
			{
				ListenToBus(controlBus);
			}
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