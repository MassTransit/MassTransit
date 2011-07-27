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
namespace MassTransit.Testing.ScenarioConfigurators
{
	using System;
	using System.Collections.Generic;
	using BusConfigurators;
	using Configurators;
	using ScenarioBuilders;

	public class BusTestScenarioBuilderConfiguratorImpl :
		ScenarioBuilderConfigurator<BusTestScenario>
	{
		readonly Action<ServiceBusConfigurator> _configureAction;

		public BusTestScenarioBuilderConfiguratorImpl(Action<ServiceBusConfigurator> configureAction)
		{
			_configureAction = configureAction;
		}

		public IEnumerable<TestConfiguratorResult> Validate()
		{
			if (_configureAction == null)
				yield return this.Failure("The scenario configuration action cannot be null");
		}

		public ScenarioBuilder<BusTestScenario> Configure(ScenarioBuilder<BusTestScenario> builder)
		{
			var busBuilder = builder as BusScenarioBuilder;
			if (busBuilder != null)
			{
				busBuilder.ConfigureBus(_configureAction);
			}

			return builder;
		}
	}
}