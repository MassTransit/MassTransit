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
	using System.Linq;
	using Builders;
	using Configurators;
	using Transports;

	public class EndpointFactoryConfiguratorImpl :
		EndpointFactoryConfigurator
	{
		readonly EndpointFactoryDefaultSettings _defaultSettings;
		readonly IList<EndpointFactoryBuilderConfigurator> _endpointFactoryConfigurators;
		Func<IEndpointFactoryDefaultSettings, EndpointFactoryBuilder> _endpointFactoryBuilderFactory;

		public EndpointFactoryConfiguratorImpl(EndpointFactoryDefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
			_endpointFactoryBuilderFactory = DefaultEndpointResolverBuilderFactory;
			_endpointFactoryConfigurators = new List<EndpointFactoryBuilderConfigurator>();
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_endpointFactoryBuilderFactory == null)
				yield return this.Failure("BuilderFactory", "The builder factory was null. Since this came from a 'Default' this is spooky.");

			foreach (var result in _endpointFactoryConfigurators.SelectMany(configurator => configurator.Validate()))
				yield return result.WithParentKey("EndpointFactory");
		}

		public void UseEndpointFactoryBuilder(Func<IEndpointFactoryDefaultSettings, EndpointFactoryBuilder> endpointFactoryBuilderFactory)
		{
			_endpointFactoryBuilderFactory = endpointFactoryBuilderFactory;
		}

		public void AddEndpointFactoryConfigurator(EndpointFactoryBuilderConfigurator configurator)
		{
			_endpointFactoryConfigurators.Add(configurator);
		}

		public IEndpointFactoryDefaultSettings Defaults
		{
			get { return _defaultSettings; }
		}

		public IEndpointFactory CreateEndpointFactory()
		{
			EndpointFactoryBuilder builder = _endpointFactoryBuilderFactory(_defaultSettings);

			foreach (EndpointFactoryBuilderConfigurator configurator in _endpointFactoryConfigurators)
			{
				builder = configurator.Configure(builder);
			}

			return builder.Build();
		}

		static EndpointFactoryBuilder DefaultEndpointResolverBuilderFactory(IEndpointFactoryDefaultSettings defaults)
		{
			return new EndpointFactoryBuilderImpl(defaults);
		}
	}
}