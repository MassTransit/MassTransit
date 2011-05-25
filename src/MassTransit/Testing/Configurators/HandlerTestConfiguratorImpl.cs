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
	using Contexts;

	public class HandlerTestConfiguratorImpl<TMessage> :
		HandlerTestConfigurator<TMessage>
		where TMessage : class
	{
		readonly IList<HandlerTestBuilderConfigurator<TMessage>> _configurators;
		readonly IList<TestContextBuilderConfigurator> _contextConfigurators;
		Func<ITestContext, HandlerTestBuilder<TMessage>> _builderFactory;

		Func<TestContextBuilder> _contextBuilderFactory;

		public HandlerTestConfiguratorImpl()
		{
			_configurators = new List<HandlerTestBuilderConfigurator<TMessage>>();
			_contextConfigurators = new List<TestContextBuilderConfigurator>();

			_builderFactory = testContext => new HandlerTestBuilderImpl<TMessage>(testContext);
			_contextBuilderFactory = () => new EndpointTestContextBuilderImpl();
		}

		public IEnumerable<TestConfiguratorResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate());
		}

		public void UseBuilder(Func<ITestContext, HandlerTestBuilder<TMessage>> builderFactory)
		{
			_builderFactory = builderFactory;
		}

		public void AddConfigurator(HandlerTestBuilderConfigurator<TMessage> configurator)
		{
			_configurators.Add(configurator);
		}

		public void UseContextBuilder(Func<TestContextBuilder> contextBuilderFactory)
		{
			_contextBuilderFactory = contextBuilderFactory;
		}

		public HandlerTest<TMessage> Build()
		{
			TestContextBuilder contextBuilder = _contextBuilderFactory();

			contextBuilder = _contextConfigurators.Aggregate(contextBuilder,
				(current, configurator) => configurator.Configure(current));

			ITestContext context = contextBuilder.Build();

			HandlerTestBuilder<TMessage> builder = _builderFactory(context);

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			return builder.Build();
		}
	}
}