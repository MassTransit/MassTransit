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
namespace MassTransit.Testing.ContextBuilders
{
	using System;
	using BusConfigurators;
	using EndpointConfigurators;
	using Magnum.Extensions;
	using TestContexts;

	public class LoopbackBusTestContextBuilderImpl :
		BusTestContextBuilder
	{
		const string DefaultUri = "loopback://localhost/mt_client";
		readonly ServiceBusConfiguratorImpl _configurator;
		readonly EndpointTestContextBuilder _endpointBuilder;
		readonly ServiceBusDefaultSettings _settings;


		public LoopbackBusTestContextBuilderImpl()
		{
			_endpointBuilder = new EndpointTestContextBuilderImpl();

			_settings = new ServiceBusDefaultSettings();
			_settings.ConcurrentConsumerLimit = 4;
			_settings.ReceiveTimeout = 50.Milliseconds();

			_configurator = new ServiceBusConfiguratorImpl(_settings);

			_configurator.ReceiveFrom(DefaultUri);
		}

		public void ConfigureEndpointFactory(Action<EndpointFactoryConfigurator> configureCallback)
		{
			_endpointBuilder.ConfigureEndpointFactory(configureCallback);
		}

		IEndpointTestContext EndpointTestContextBuilder.Build()
		{
			IEndpointTestContext endpointContext = _endpointBuilder.Build();

			return endpointContext;
		}

		public void ConfigureServiceBus(Action<ServiceBusConfigurator> configureCallback)
		{
			configureCallback(_configurator);
		}

		public IBusTestContext Build()
		{
			IEndpointTestContext endpointContext = _endpointBuilder.Build();

			_settings.EndpointCache = endpointContext.EndpointCache;

			_configurator.ChangeSettings(x => { x.EndpointCache = endpointContext.EndpointCache; });

			IServiceBus bus = _configurator.CreateServiceBus();

			var context = new BusTestContext(endpointContext, bus);

			return context;
		}
	}
}