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
namespace MassTransit.EndpointConfigurators
{
	using System;
	using System.Collections.Generic;
	using Configurators;
	using MassTransit.Builders;
	using MassTransit.Exceptions;
	using MassTransit.Transports;

	public class EndpointFactoryConfiguratorImpl :
		EndpointFactoryConfigurator
	{
		readonly IList<EndpointFactoryBuilderConfigurator> _endpointFactoryConfigurators;
		Func<EndpointFactoryBuilder> _endpointFactoryBuilderFactory;

		public EndpointFactoryConfiguratorImpl()
		{
			_endpointFactoryBuilderFactory = DefaultEndpointResolverBuilderFactory;
			_endpointFactoryConfigurators = new List<EndpointFactoryBuilderConfigurator>();
		}

		public void Validate()
		{
			if (_endpointFactoryBuilderFactory == null)
				throw new ConfigurationException("The endpoint resolver builder factory can not be null.");

			_endpointFactoryConfigurators.Each(configurator => configurator.Validate());
		}

		public void UseEndpointFactoryBuilder(Func<EndpointFactoryBuilder> endpointFactoryBuilderFactory)
		{
			_endpointFactoryBuilderFactory = endpointFactoryBuilderFactory;
		}

		public void AddEndpointFactoryConfigurator(EndpointFactoryBuilderConfigurator configurator)
		{
			_endpointFactoryConfigurators.Add(configurator);
		}

		public IEndpointFactory CreateEndpointFactory()
		{
			EndpointFactoryBuilder builder = _endpointFactoryBuilderFactory();

			foreach (EndpointFactoryBuilderConfigurator configurator in _endpointFactoryConfigurators)
			{
				builder = configurator.Configure(builder);
			}

			return builder.Build();
		}

		static EndpointFactoryBuilder DefaultEndpointResolverBuilderFactory()
		{
			return new EndpointFactoryBuilderImpl();
		}
	}
}