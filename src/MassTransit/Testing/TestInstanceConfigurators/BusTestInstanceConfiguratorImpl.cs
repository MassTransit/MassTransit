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
	using ContextBuilders;
	using ContextConfigurators;
	using Magnum.Extensions;
	using TestContexts;

	public abstract class BusTestInstanceConfiguratorImpl
	{
		readonly IList<TestActionConfigurator> _actionConfigurators;
		readonly IList<BusTestContextBuilderConfigurator> _configurators;
		Func<BusTestContextBuilder> _builderFactory;

		protected BusTestInstanceConfiguratorImpl()
		{
			_configurators = new List<BusTestContextBuilderConfigurator>();
			_actionConfigurators = new List<TestActionConfigurator>();

			_builderFactory = () => new LoopbackBusTestContextBuilder();
		}

		public void AddActionConfigurator(TestActionConfigurator action)
		{
			_actionConfigurators.Add(action);
		}

		public void UseContextBuilder(Func<BusTestContextBuilder> contextBuilderFactory)
		{
			_builderFactory = contextBuilderFactory;
		}

		public void AddConfigurator(BusTestContextBuilderConfigurator configurator)
		{
			_configurators.Add(configurator);
		}

		public virtual IEnumerable<TestConfiguratorResult> Validate()
		{
			return _configurators.SelectMany(x => x.Validate())
				.Concat(_actionConfigurators.SelectMany(x => x.Validate()));
		}

		protected BusTestContext BuildBusTextContext()
		{
			BusTestContextBuilder contextBuilder = _builderFactory();

			contextBuilder = _configurators.Aggregate(contextBuilder,
				(current, configurator) => configurator.Configure(current));

			BusTestContext context = contextBuilder.Build();

			return context;
		}

		protected void BuildTestActions(TestInstanceBuilder builder)
		{
			_actionConfigurators.Each(x => x.Configure(builder));
		}
	}
}