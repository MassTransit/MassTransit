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

	public class ConsumerTestInstanceConfiguratorImpl<TConsumer> :
		BusTestInstanceConfiguratorImpl,
		ConsumerTestInstanceConfigurator<TConsumer>
		where TConsumer : class
	{
		readonly IList<ConsumerTestBuilderConfigurator<TConsumer>> _configurators;

		Func<BusTestScenario, ConsumerTestBuilder<TConsumer>> _builderFactory;
		IConsumerFactory<TConsumer> _consumerFactory;

		public ConsumerTestInstanceConfiguratorImpl()
		{
			_configurators = new List<ConsumerTestBuilderConfigurator<TConsumer>>();

			_builderFactory = testContext => new ConsumerTestBuilderImpl<TConsumer>(testContext);
		}

		public void UseBuilder(Func<BusTestScenario, ConsumerTestBuilder<TConsumer>> builderFactory)
		{
			_builderFactory = builderFactory;
		}

		public void AddConfigurator(ConsumerTestBuilderConfigurator<TConsumer> configurator)
		{
			_configurators.Add(configurator);
		}

		public void UseConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
		{
			_consumerFactory = consumerFactory;
		}

		public IEnumerable<TestConfiguratorResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate());
		}

		public ConsumerTest<TConsumer> Build()
		{
			BusTestScenario context = BuildBusTestScenario();

			ConsumerTestBuilder<TConsumer> builder = _builderFactory(context);

			builder.SetConsumerFactory(_consumerFactory);

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			BuildTestActions(builder);

			return builder.Build();
		}
	}
}