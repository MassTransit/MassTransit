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
	using Builders;
	using Configurators;
	using Transports;

	/// <summary>
	/// Allows for the configuration of the EndpointFactory through the use of an EndpointFactoryConfigurator
	/// </summary>
	public interface EndpointFactoryConfigurator :
		Configurator
	{
		/// <summary>
		/// Gets the endpoint factory defaults.
		/// </summary>
		IEndpointFactoryDefaultSettings Defaults { get; }

		/// <summary>
		/// Creates the endpoint factory with the configuration
		/// </summary>
		/// <returns></returns>
		IEndpointFactory CreateEndpointFactory();

		/// <summary>
		/// Overrides the default EndpointResolver builder with another builder
		/// </summary>
		/// <param name="endpointFactoryBuilderFactory"></param>
		void UseEndpointFactoryBuilder(Func<IEndpointFactoryDefaultSettings, EndpointFactoryBuilder> endpointFactoryBuilderFactory);

		/// <summary>
		/// Adds an endpoint configurator to the endpoint resolver builder
		/// </summary>
		/// <param name="configurator"></param>
		void AddEndpointFactoryConfigurator(EndpointFactoryBuilderConfigurator configurator);
	}
}