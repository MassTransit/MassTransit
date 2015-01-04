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
	using BuilderConfigurators;
	using Builders;
	using Configurators;
	using Saga;
	using ScenarioBuilders;
	using Scenarios;

	public class SagaTestConfigurator<TScenario, TSaga> :
		TestConfigurator<TScenario>,
		ISagaTestConfigurator<TScenario, TSaga>
		where TSaga : class, ISaga
		where TScenario : IBusTestScenario
	{
		readonly IList<ISagaTestSpecification<TScenario, TSaga>> _configurators;

		Func<TScenario, SagaTestBuilder<TScenario, TSaga>> _builderFactory;
		ISagaRepository<TSaga> _sagaRepository;

		public SagaTestConfigurator(Func<ITestScenarioBuilder<TScenario>> scenarioBuilderFactory)
			: base(scenarioBuilderFactory)
		{
			_configurators = new List<ISagaTestSpecification<TScenario, TSaga>>();

			_builderFactory = scenario => new SagaTestBuilderImpl<TScenario, TSaga>(scenario);
		}

		public void UseBuilder(Func<TScenario, SagaTestBuilder<TScenario, TSaga>> builderFactory)
		{
			_builderFactory = builderFactory;
		}

		public void AddConfigurator(ISagaTestSpecification<TScenario, TSaga> configurator)
		{
			_configurators.Add(configurator);
		}

		public void UseSagaRepository(ISagaRepository<TSaga> sagaRepository)
		{
			_sagaRepository = sagaRepository;
		}

		public new IEnumerable<TestConfiguratorResult> Validate()
		{
			return base.Validate().Concat(_configurators.SelectMany(x => x.Validate()));
		}

		public SagaTest<TScenario, TSaga> Build()
		{
			TScenario context = BuildTestScenario();

			SagaTestBuilder<TScenario, TSaga> builder = _builderFactory(context);

			builder.SetSagaRepository(_sagaRepository);

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			BuildTestActions(builder);

			return builder.Build();
		}
	}
}