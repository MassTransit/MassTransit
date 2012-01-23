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
namespace MassTransit.Testing
{
	using Configurators;
	using ScenarioConfigurators;

	/// <summary>
	/// Extension methods for the bus test scenarios.
	/// </summary>
	public static class BusTestScenarioExtensions
	{
		/// <summary>
		/// Sets the concurrent consumer limit for the <see cref="BusTestScenario"/> that is
		/// under test.
		/// </summary>
		/// <param name="configurator">The configurator passed from the XXXTestFactory interfaces' "New(HandlerTestInstanceConfigurator{TScenario,TMessage})" method.</param>
		/// <param name="value">The value for this setting.</param>
		public static void SetConcurrentConsumerLimit(this ScenarioConfigurator<BusTestScenario> configurator, int value)
		{
			var scenarioConfigurator =
				new BusTestScenarioBuilderConfiguratorImpl(x => x.SetConcurrentConsumerLimit(value));

			configurator.AddConfigurator(scenarioConfigurator);
		}
	}
}