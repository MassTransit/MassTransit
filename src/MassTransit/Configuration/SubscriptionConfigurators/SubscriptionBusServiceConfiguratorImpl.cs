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
	using Subscriptions;

	/// <summary>
	/// Handles the configuration of subscriptions as part of the bus
	/// 
	/// </summary>
	public class SubscriptionBusServiceConfiguratorImpl :
		SubscriptionBusServiceConfigurator,
		BusServiceConfigurator,
		BusBuilderConfigurator
	{
		readonly IList<SubscriptionBusServiceBuilderConfigurator> _configurators;

		public SubscriptionBusServiceConfiguratorImpl()
		{
			_configurators = new List<SubscriptionBusServiceBuilderConfigurator>();
		}

		public IEnumerable<ValidationResult> Validate()
		{
			return from configurator in _configurators
			       from result in configurator.Validate()
			       select result.WithParentKey("Subscribe");
		}

		public void AddConfigurator(SubscriptionBusServiceBuilderConfigurator configurator)
		{
			_configurators.Add(configurator);
		}

		public BusBuilder Configure(BusBuilder builder)
		{
			builder.AddBusServiceConfigurator(this);

			return builder;
		}

		public Type ServiceType
		{
			get { return typeof (SubscriptionBusService); }
		}

		public BusServiceLayer Layer
		{
			get { return BusServiceLayer.Application; }
		}

		public IBusService Create(IServiceBus bus)
		{
			var subscriptionServiceBuilder = new SubscriptionBusServiceBuilderImpl();

			foreach (SubscriptionBusServiceBuilderConfigurator configurator in _configurators)
			{
				configurator.Configure(subscriptionServiceBuilder);
			}

			return subscriptionServiceBuilder.Build();
		}
	}
}