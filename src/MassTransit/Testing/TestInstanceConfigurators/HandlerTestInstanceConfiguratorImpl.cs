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
	using Scenarios;

	public class HandlerTestInstanceConfiguratorImpl<TMessage> :
		BusTestInstanceConfiguratorImpl,
		HandlerTestInstanceConfigurator<TMessage>
		where TMessage : class
	{
		readonly IList<HandlerTestBuilderConfigurator<TMessage>> _configurators;

		Func<BusTestScenario, HandlerTestBuilder<TMessage>> _builderFactory;
		Action<IServiceBus, TMessage> _handler;

		public HandlerTestInstanceConfiguratorImpl()
		{
			_configurators = new List<HandlerTestBuilderConfigurator<TMessage>>();

			_builderFactory = testContext => new HandlerTestBuilderImpl<TMessage>(testContext);
		}

		public void UseBuilder(Func<BusTestScenario, HandlerTestBuilder<TMessage>> builderFactory)
		{
			_builderFactory = builderFactory;
		}

		public void AddConfigurator(HandlerTestBuilderConfigurator<TMessage> configurator)
		{
			_configurators.Add(configurator);
		}

		public void Handler(Action<IServiceBus, TMessage> handler)
		{
			_handler = handler;
		}

		public override IEnumerable<TestConfiguratorResult> Validate()
		{
			return base.Validate().Concat(_configurators.SelectMany(x => x.Validate()));
		}

		public HandlerTest<TMessage> Build()
		{
			BusTestScenario scenario = BuildBusTestScenario();

			HandlerTestBuilder<TMessage> builder = _builderFactory(scenario);

			if (_handler != null)
				builder.SetHandler(_handler);

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			BuildTestActions(builder);

			return builder.Build();
		}
	}
}