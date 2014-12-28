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
namespace MassTransit.Testing.TestInstanceConfigurators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ActionConfigurators;
	using Builders;
	using Configurators;
	using ScenarioBuilders;
	using ScenarioConfigurators;
	using Scenarios;

	public abstract class TestInstanceConfiguratorImpl<TScenario>
		where TScenario : ITestScenario
	{
		readonly IList<ITestActionConfigurator<TScenario>> _actionConfigurators;
		readonly IList<IScenarioBuilderConfigurator<TScenario>> _configurators;
		Func<ITestScenarioBuilder<TScenario>> _builderFactory;

		protected TestInstanceConfiguratorImpl(Func<ITestScenarioBuilder<TScenario>> scenarioBuilderFactory)
		{
			_configurators = new List<IScenarioBuilderConfigurator<TScenario>>();
			_actionConfigurators = new List<ITestActionConfigurator<TScenario>>();

			_builderFactory = scenarioBuilderFactory;
		}

		public void AddActionConfigurator(ITestActionConfigurator<TScenario> action)
		{
			_actionConfigurators.Add(action);
		}

		public void UseScenarioBuilder(Func<ITestScenarioBuilder<TScenario>> contextBuilderFactory)
		{
			_builderFactory = contextBuilderFactory;
		}

		public void AddConfigurator(IScenarioBuilderConfigurator<TScenario> configurator)
		{
			_configurators.Add(configurator);
		}

		public virtual IEnumerable<TestConfiguratorResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate())
				.Concat(_actionConfigurators.SelectMany(x => x.Validate()));
		}

		protected TScenario BuildTestScenario()
		{
			ITestScenarioBuilder<TScenario> builder = _builderFactory();

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			TScenario context = builder.Build();

			return context;
		}

		protected void BuildTestActions(TestInstanceBuilder<TScenario> builder)
		{
		    foreach (var configurator in _actionConfigurators)
		    {
		        configurator.Configure(builder);
		    }
		}
	}
}