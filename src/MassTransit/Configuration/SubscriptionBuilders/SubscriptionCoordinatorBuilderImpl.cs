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
namespace MassTransit.SubscriptionBuilders
{
	using System;
	using System.Collections.Generic;
	using Subscriptions.Actors;

	public class SubscriptionCoordinatorBuilderImpl :
		SubscriptionCoordinatorBuilder
	{
		readonly IServiceBus _bus;
		string _network;
		readonly IList<Func<IServiceBus, BusSubscriptionEventObserver>> _observers;

		public SubscriptionCoordinatorBuilderImpl(IServiceBus bus, string network)
		{
			_bus = bus;
			_network = network;
			_observers = new List<Func<IServiceBus, BusSubscriptionEventObserver>>();
		}

		public SubscriptionCoordinatorBusService Build()
		{
			if(_observers.Count == 0)
				_observers.Add(bus => new BusSubscriptionConnector(bus));

			return new SubscriptionCoordinatorBusService(_network, _observers);
		}

		public void SetObserverFactory(Func<IServiceBus, BusSubscriptionEventObserver> observerFactory)
		{
			_observers.Clear();
			_observers.Add(observerFactory);
		}
	}
}