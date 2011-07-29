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
namespace MassTransit.SubscriptionConfigurators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Builders;
	using BusConfigurators;
	using BusServiceConfigurators;
	using Configurators;
	using SubscriptionBuilders;
	using Subscriptions.Actors;

	public class SubscriptionCoordinatorConfiguratorImpl :
		SubscriptionCoordinatorConfigurator,
		BusServiceConfigurator,
		BusBuilderConfigurator
	{
		readonly IList<SubscriptionCoordinatorBuilderConfigurator> _configurators;
		string _network;

		public SubscriptionCoordinatorConfiguratorImpl()
		{
			_configurators = new List<SubscriptionCoordinatorBuilderConfigurator>();
		}

		public IEnumerable<ValidationResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate());
		}

		public BusBuilder Configure(BusBuilder builder)
		{
			builder.Match<ServiceBusBuilder>(x => x.AddBusServiceConfigurator(this));

			return builder;
		}

		public Type ServiceType
		{
			get { return typeof (SubscriptionCoordinatorBusService); }
		}

		public BusServiceLayer Layer
		{
			get { return BusServiceLayer.Session; }
		}

		public IBusService Create(IServiceBus bus)
		{
			SubscriptionCoordinatorBuilder builder = new SubscriptionCoordinatorBuilderImpl(bus, _network);

			builder = _configurators.Aggregate(builder, (seed, next) => next.Configure(seed));

			return builder.Build();
		}

		public void SetNetwork(string network)
		{
			_network = network;
		}

		public void AddConfigurator(SubscriptionCoordinatorBuilderConfigurator configurator)
		{
			_configurators.Add(configurator);
		}
	}
}