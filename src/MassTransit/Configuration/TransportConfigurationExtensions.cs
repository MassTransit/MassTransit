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
	using EndpointConfigurators;

	public static class TransportConfigurationExtensions
	{
		public static EndpointFactoryConfigurator SetCreateMissingQueues(this EndpointFactoryConfigurator configurator,
		                                                                 bool value)
		{
			var builderConfigurator = new DelegateEndpointFactoryBuilderConfigurator(x => x.SetCreateMissingQueues(value));

			configurator.AddEndpointFactoryConfigurator(builderConfigurator);

			return configurator;
		}

		public static EndpointFactoryConfigurator SetCreateTransactionalQueues(this EndpointFactoryConfigurator configurator,
		                                                                       bool value)
		{
			var builderConfigurator = new DelegateEndpointFactoryBuilderConfigurator(x => x.SetCreateTransactionalQueues(value));

			configurator.AddEndpointFactoryConfigurator(builderConfigurator);

			return configurator;
		}

		public static EndpointFactoryConfigurator SetPurgeOnStartup(this EndpointFactoryConfigurator configurator, bool value)
		{
			var builderConfigurator = new DelegateEndpointFactoryBuilderConfigurator(x => x.SetPurgeOnStartup(value));

			configurator.AddEndpointFactoryConfigurator(builderConfigurator);

			return configurator;
		}
	}
}