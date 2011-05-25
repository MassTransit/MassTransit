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
namespace MassTransit.Testing.Configurators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Builders;
	using ContextBuilders;
	using ContextConfigurators;
	using Magnum.Extensions;
	using TestContexts;

	public class ConsumerTestConfiguratorImpl<TConsumer> :
		ConsumerTestConfigurator<TConsumer>
		where TConsumer : class
	{
		readonly IList<ConsumerTestBuilderConfigurator<TConsumer>> _configurators;
		readonly IList<BusTestContextBuilderConfigurator> _contextConfigurators;
		readonly IList<TestActionConfigurator> _actionConfigurators;

		Func<IBusTestContext, ConsumerTestBuilder<TConsumer>> _builderFactory;
		Func<BusTestContextBuilder> _contextBuilderFactory;
		IConsumerFactory<TConsumer> _consumerFactory;

		public ConsumerTestConfiguratorImpl()
		{
			_configurators = new List<ConsumerTestBuilderConfigurator<TConsumer>>();
			_contextConfigurators = new List<BusTestContextBuilderConfigurator>();
			_actionConfigurators = new List<TestActionConfigurator>();

			_builderFactory = testContext => new ConsumerTestBuilderImpl<TConsumer>(testContext);
			_contextBuilderFactory = () => new LoopbackBusTestContextBuilderImpl();
		}

		public void UseBuilder(Func<IBusTestContext, ConsumerTestBuilder<TConsumer>> builderFactory)
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

		public void AddActionConfigurator(TestActionConfigurator action)
		{
			_actionConfigurators.Add(action);
		}

		public IEnumerable<TestConfiguratorResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate());
		}

		public void UseContextBuilder(Func<BusTestContextBuilder> contextBuilderFactory)
		{
			_contextBuilderFactory = contextBuilderFactory;
		}

		public ConsumerTest<TConsumer> Build()
		{
			BusTestContextBuilder contextBuilder = _contextBuilderFactory();

			contextBuilder = _contextConfigurators.Aggregate(contextBuilder,
				(current, configurator) => configurator.Configure(current));

			IBusTestContext context = contextBuilder.Build();

			ConsumerTestBuilder<TConsumer> builder = _builderFactory(context);

			builder.SetConsumerFactory(_consumerFactory);

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			_actionConfigurators.Each(x => x.Configure(builder));

			return builder.Build();
		}
	}
}