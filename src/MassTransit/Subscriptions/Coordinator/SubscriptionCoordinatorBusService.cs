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
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using Stact;

	public class SubscriptionCoordinatorBusService :
		IBusService,
		BusSubscriptionCoordinator,
		BusSubscriptionEventObserver
	{
		readonly Guid _clientId;
		readonly string _network;
		readonly IList<BusSubscriptionEventObserver> _observers;
		IServiceBus _bus;
		IServiceBus _controlBus;
		Uri _controlUri;
		Uri _dataUri;

		bool _disposed;
		ActorInstance _peerCache;
		UnsubscribeAction _unregister;

		public SubscriptionCoordinatorBusService(IServiceBus bus, string network)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_dataUri = bus.Endpoint.Address.Uri;
			_controlUri = bus.ControlBus.Endpoint.Address.Uri;

			_network = network;

			_clientId = CombGuid.Generate();

			_observers = new List<BusSubscriptionEventObserver>();

			_unregister = () => true;

			_bus = bus;
			_controlBus = bus.ControlBus;

			_dataUri = bus.Endpoint.Address.Uri;
			_controlUri = bus.ControlBus.Endpoint.Address.Uri;

			var connector = new BusSubscriptionConnector(bus);

			_peerCache = ActorFactory.Create<PeerCache>(x =>
				{
					x.ConstructedBy((fiber, scheduler, inbox) =>
					                new PeerCache(fiber, scheduler, connector, _clientId, _controlUri));
					x.UseSharedScheduler();
					x.HandleOnPoolFiber();
				})
			.GetActor();
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

		public string Network
		{
			get { return _network; }
		}

		public Guid ClientId
		{
			get { return _clientId; }
		}

		public Uri ControlUri
		{
			get { return _controlUri; }
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

		public void OnComplete()
		{
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			ListenToBus(bus);
		}

		public void Stop()
		{
			_unregister();
		}

		public void AddObserver(BusSubscriptionEventObserver observer)
		{
			lock (_observers)
				_observers.Add(observer);
		}

		void ListenToBus(IServiceBus bus)
		{
			var subscriptionEventListener = new BusSubscriptionEventListener(bus, this);

			_unregister += bus.Configure(x =>
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
				lock (_observers)
					_observers.Each(x => x.OnComplete());
			}

			_disposed = true;
		}

		~SubscriptionCoordinatorBusService()
		{
			Dispose(false);
		}
	}
}