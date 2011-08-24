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
	using Subscriptions.Coordinator;

	public class SubscriptionRouterConfiguratorImpl :
		BusServiceConfigurator,
		BusBuilderConfigurator
	{
		readonly IList<SubscriptionRouterBuilderConfigurator> _configurators;
		string _network;

		public SubscriptionRouterConfiguratorImpl(string network)
		{
			_configurators = new List<SubscriptionRouterBuilderConfigurator>();
			_network = network;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate());
		}

		public BusBuilder Configure(BusBuilder builder)
		{
			builder.AddBusServiceConfigurator(this);

			return builder;
		}

		public Type ServiceType
		{
			get { return typeof (SubscriptionRouterService); }
		}

		public BusServiceLayer Layer
		{
			get { return BusServiceLayer.Session; }
		}

		public IBusService Create(IServiceBus bus)
		{
			SubscriptionRouterBuilder builder = new SubscriptionRouterBuilderImpl(bus, _network);

			builder = _configurators.Aggregate(builder, (seed, next) => next.Configure(seed));

			return builder.Build();
		}

		public void SetNetwork(string network)
		{
			_network = network;
		}

		public void AddConfigurator(SubscriptionRouterBuilderConfigurator configurator)
		{
			_configurators.Add(configurator);
		}
	}
}