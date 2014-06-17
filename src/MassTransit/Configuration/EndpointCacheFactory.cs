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
namespace MassTransit
{
	using System;
	using Configurators;
	using EndpointConfigurators;
	using Exceptions;
	using Magnum;
	using Transports;
	using Util;

	public static class EndpointCacheFactory
	{
		static readonly EndpointFactoryDefaultSettings _defaultSettings = new EndpointFactoryDefaultSettings();

		
		public static IEndpointCache New( Action<EndpointFactoryConfigurator> configure)
		{
			Guard.AgainstNull(configure, "configure");

			var configurator = new EndpointFactoryConfiguratorImpl(_defaultSettings);

			configure(configurator);

			ConfigurationResult result = ConfigurationResultImpl.CompileResults(configurator.Validate());

			try
			{
				IEndpointFactory endpointFactory = configurator.CreateEndpointFactory();

				IEndpointCache endpointCache = new EndpointCache(endpointFactory);

				return endpointCache;
			}
			catch (Exception ex)
			{
				throw new ConfigurationException(result, "An exception was thrown during endpoint cache creation", ex);
			}
		}

		public static void ConfigureDefaultSettings( Action<EndpointFactoryDefaultSettingsConfigurator> configure)
		{
			Guard.AgainstNull(configure);

			var configurator = new EndpointFactoryDefaultSettingsConfiguratorImpl(_defaultSettings);

			configure(configurator);
		}
	}
}