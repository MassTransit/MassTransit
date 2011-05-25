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
	using EndpointConfigurators;
	using Exceptions;
	using Magnum.Extensions;
	using MassTransit.Configurators;
	using Serialization;
	using TestContexts;
	using Transports;
	using Transports.Loopback;

	public class EndpointTestContextBuilderImpl :
		EndpointTestContextBuilder
	{
		readonly EndpointFactoryConfigurator _endpointFactoryConfigurator;

		public EndpointTestContextBuilderImpl()
		{
			var settings = new EndpointFactoryDefaultSettings();

			settings.CreateMissingQueues = true;
			settings.CreateTransactionalQueues = false;
			settings.PurgeOnStartup = true;
			settings.RequireTransactional = false;
			settings.Serializer = new XmlMessageSerializer();
			settings.TransactionTimeout = 30.Seconds();

			_endpointFactoryConfigurator = new EndpointFactoryConfiguratorImpl(settings);

			_endpointFactoryConfigurator.AddTransportFactory<LoopbackTransportFactory>();
		}

		public void ConfigureEndpointFactory(Action<EndpointFactoryConfigurator> configureCallback)
		{
			configureCallback(_endpointFactoryConfigurator);
		}

		public IEndpointTestContext Build()
		{
			ConfigurationResult result = ConfigurationResultImpl.CompileResults(_endpointFactoryConfigurator.Validate());

			try
			{
				IEndpointFactory endpointFactory = _endpointFactoryConfigurator.CreateEndpointFactory();

				var context = new EndpointTestContext(endpointFactory);

				return context;
			}
			catch (Exception ex)
			{
				throw new ConfigurationException(result, "An exception was thrown during endpoint cache creation", ex);
			}
		}
	}
}