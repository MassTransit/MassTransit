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
	using ScenarioBuilders;
	using Scenarios;

	public class HandlerTestInstanceConfiguratorImpl<TScenario, TMessage> :
		TestInstanceConfiguratorImpl<TScenario>,
		HandlerTestInstanceConfigurator<TScenario, TMessage>
		where TMessage : class
		where TScenario : ITestScenario
	{
		readonly IList<HandlerTestBuilderConfigurator<TScenario, TMessage>> _configurators;

		Func<TScenario, HandlerTestBuilder<TScenario, TMessage>> _builderFactory;
		MessageHandler<TMessage> _handler;

		public HandlerTestInstanceConfiguratorImpl(Func<ITestScenarioBuilder<TScenario>> scenarioBuilderFactory)
			: base(scenarioBuilderFactory)
		{
			_configurators = new List<HandlerTestBuilderConfigurator<TScenario, TMessage>>();

			_builderFactory = scenario => new HandlerTestBuilderImpl<TScenario, TMessage>(scenario);
		}

		public void UseBuilder(Func<TScenario, HandlerTestBuilder<TScenario, TMessage>> builderFactory)
		{
			_builderFactory = builderFactory;
		}

		public void AddConfigurator(HandlerTestBuilderConfigurator<TScenario, TMessage> configurator)
		{
			_configurators.Add(configurator);
		}

		public void Handler(MessageHandler<TMessage> handler)
		{
			_handler = handler;
		}

		public override IEnumerable<TestConfiguratorResult> Validate()
		{
			return base.Validate().Concat(_configurators.SelectMany(x => x.Validate()));
		}

		public HandlerTest<TScenario, TMessage> Build()
		{
			TScenario scenario = BuildTestScenario();

			HandlerTestBuilder<TScenario, TMessage> builder = _builderFactory(scenario);

			if (_handler != null)
				builder.SetHandler(_handler);

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			BuildTestActions(builder);

			return builder.Build();
		}
	}
}