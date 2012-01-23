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
	using Scenarios;
	using TestInstanceConfigurators;

	/// <summary>
	/// Extensions for configuring a test RabbitMQ instance that can be used
	/// in the test. See <see cref="RabbitMqBusScenarioBuilder"/> docs.
	/// </summary>
	public static class BusTestScenarioExtensions
	{
		/// <summary>
		/// Extensions for configuring a test RabbitMQ instance that can be used
		/// in the test. See <see cref="RabbitMqBusScenarioBuilder"/> docs.
		/// </summary>
		/// <param name="configurator"></param>
		public static void UseRabbitMqBusScenario(this TestInstanceConfigurator<BusTestScenario> configurator)
		{
			configurator.UseScenarioBuilder(() => new RabbitMqBusScenarioBuilder());
		}
	}
}