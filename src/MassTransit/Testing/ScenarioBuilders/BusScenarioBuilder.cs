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
	using SubscriptionConfigurators;

	public interface BusScenarioBuilder :
		EndpointScenarioBuilder<BusTestScenario>
	{
		/// <summary>
		/// Configure any bus-specific items as part of building the test scenario
		/// </summary>
		/// <param name="configureCallback"></param>
		void ConfigureBus(Action<ServiceBusConfigurator> configureCallback);

		/// <summary>
		/// Configure the subscriptions for a test using this scenario
		/// </summary>
		/// <param name="configureCallback"></param>
		void ConfigureSubscriptions(Action<SubscriptionBusServiceConfigurator> configureCallback);
	}
}