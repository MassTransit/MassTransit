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
	using Diagnostics.Introspection;
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using Stact;

	public class SubscriptionRouterService :
		IBusService,
		SubscriptionRouter,
		SubscriptionObserver,
        DiagnosticsSource
	{
		readonly Guid _peerId;
		readonly Uri _peerUri;
		readonly IList<BusSubscriptionEventListener> _listeners;
		readonly string _network;
		readonly IList<SubscriptionObserver> _observers;
        readonly ActorRef _peerCache;
		bool _disposed;
		UnsubscribeAction _unregister;

		public SubscriptionRouterService(IServiceBus bus, string network)
		{
			_peerUri = bus.ControlBus.Endpoint.Address.Uri;

			_network = network;

			_peerId = CombGuid.Generate();

			_observers = new List<SubscriptionObserver>();
			_listeners = new List<BusSubscriptionEventListener>();

			_unregister = () => true;

			_peerUri = bus.ControlBus.Endpoint.Address.Uri;

			var connector = new BusSubscriptionConnector(bus);

			_peerCache = ActorFactory.Create<PeerCache>(x =>
				{
					x.ConstructedBy((fiber, scheduler, inbox) =>
					                new PeerCache(fiber, scheduler, connector, _peerId, _peerUri));
					x.UseSharedScheduler();
					x.HandleOnPoolFiber();
				})
				.GetActor();
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

		public IEnumerable<Subscription> LocalSubscriptions
		{
			get { return _listeners.SelectMany(x => x.Subscriptions); }
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

		public Guid PeerId
		{
			get { return _peerId; }
		}

		public Uri PeerUri
		{
			get { return _peerUri; }
		}

		public void AddObserver(SubscriptionObserver observer)
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

			_listeners.Add(subscriptionEventListener);

			IServiceBus controlBus = bus.ControlBus;
			if (controlBus != bus)
			{
				ListenToBus(controlBus);
			}
		}

	    public void Inspect(DiagnosticsProbe probe)
	    {
	        probe.Add("mt.network",_network);
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

		~SubscriptionRouterService()
		{
			Dispose(false);
		}
	}
}