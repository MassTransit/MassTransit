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
namespace MassTransit.Testing.ScenarioBuilders
{
	using System;
	using BusConfigurators;
	using Magnum.Extensions;
	using Scenarios;
	using SubscriptionConfigurators;
	using Transports;

	/// <summary>
	/// Implementation for the test scenario, but abstract for others to customize it. Sets some defaults in the c'tor, which you
	/// can override with the <see cref="ConfigureBus"/> and <see cref="ConfigureSubscriptions"/> methods.
	/// </summary>
	public abstract class BusScenarioBuilderImpl :
		EndpointScenarioBuilderImpl<BusTestScenario>,
		BusScenarioBuilder
	{
		readonly ServiceBusConfiguratorImpl _configurator;
		readonly ServiceBusDefaultSettings _settings;

		/// <summary>
		/// c'tor
		/// </summary>
		/// <param name="uri">The uri to receive from during the scenario.</param>
		protected BusScenarioBuilderImpl(Uri uri)
		{
			_settings = new ServiceBusDefaultSettings();
			_settings.ConcurrentConsumerLimit = 1;
			_settings.ReceiveTimeout = 50.Milliseconds();

			_configurator = new ServiceBusConfiguratorImpl(_settings);
			_configurator.ReceiveFrom(uri);
		}

		public void ConfigureBus(Action<ServiceBusConfigurator> configureCallback)
		{
			configureCallback(_configurator);
		}

		public void ConfigureSubscriptions(Action<SubscriptionBusServiceConfigurator> configureCallback)
		{
			_configurator.Subscribe(configureCallback);
		}

		public override BusTestScenario Build()
		{
			IEndpointFactory endpointFactory = BuildEndpointFactory();

			var scenario = new BusTestScenarioImpl(endpointFactory);

			_configurator.ChangeSettings(x => { x.EndpointCache = scenario.EndpointCache; });
			_configurator.EnableMessageTracing();

			scenario.Bus = _configurator.CreateServiceBus();

			return scenario;
		}
	}
}