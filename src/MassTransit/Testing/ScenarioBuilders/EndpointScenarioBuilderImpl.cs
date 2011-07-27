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
	using EndpointConfigurators;
	using Exceptions;
	using Magnum.Extensions;
	using MassTransit.Configurators;
	using Scenarios;
	using Serialization;
	using Transports;
	using Transports.Loopback;

	public abstract class EndpointScenarioBuilderImpl<TScenario> :
		EndpointScenarioBuilder<TScenario>
		where TScenario : TestScenario
	{
		readonly EndpointFactoryConfigurator _endpointFactoryConfigurator;

		public EndpointScenarioBuilderImpl()
		{
			var settings = new EndpointFactoryDefaultSettings
				{
					CreateMissingQueues = true,
					CreateTransactionalQueues = false,
					PurgeOnStartup = true,
					RequireTransactional = false,
					Serializer = new XmlMessageSerializer(),
					TransactionTimeout = 30.Seconds()
				};

			_endpointFactoryConfigurator = new EndpointFactoryConfiguratorImpl(settings);

			_endpointFactoryConfigurator.AddTransportFactory<LoopbackTransportFactory>();
		}

		public void ConfigureEndpointFactory(Action<EndpointFactoryConfigurator> configureCallback)
		{
			configureCallback(_endpointFactoryConfigurator);
		}

		protected IEndpointFactory BuildEndpointFactory()
		{
			ConfigurationResult result = ConfigurationResultImpl.CompileResults(_endpointFactoryConfigurator.Validate());

			IEndpointFactory endpointFactory;
			try
			{
				endpointFactory = _endpointFactoryConfigurator.CreateEndpointFactory();
			}
			catch (Exception ex)
			{
				throw new ConfigurationException(result, "An exception was thrown during endpoint cache creation", ex);
			}
			return endpointFactory;
		}

		public abstract TScenario Build();
	}
}