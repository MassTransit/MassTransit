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
	using Scenarios;

	/// <summary>
	/// And endpoint scenario builder implementation ties together the scenario 
	/// with the underlying infrastructure.
	/// </summary>
	/// <typeparam name="TScenario">See <see cref="BusTestScenario"/>, <see cref="EndpointTestScenario"/> and <see cref="TestScenario"/>
	/// for feeding as the generic parameter.</typeparam>
	public interface EndpointScenarioBuilder<TScenario> :
		ScenarioBuilder<TScenario>
		where TScenario : TestScenario
	{
		/// <summary>
		/// Endpoint scenario builders may call this method to configure the endpoint factory. Call this method
		/// to customize how the endpoint uris are built. Example:
		/// <code>
		/// ConfigureEndpointFactory(x =>
		///    {
		///    	x.UseRabbitMq();
		///    });
		/// </code>
		/// </summary>
		/// <param name="configureCallback"></param>
		void ConfigureEndpointFactory(Action<EndpointFactoryConfigurator> configureCallback);
	}
}