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
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Configurators;
	using ContextBuilders;
	using Contexts;

	public class TestContextConfiguratorImpl :
		TestContextConfigurator
	{
		Func<TestContextBuilder> _builderFactory;
		IList<TestContextBuilderConfigurator> _configurators;

		public TestContextConfiguratorImpl()
		{
			_configurators = new List<TestContextBuilderConfigurator>();
		}

		public IEnumerable<TestConfiguratorResult> Validate()
		{
			if (_builderFactory == null)
				yield return this.Failure("BuilderFactory", "A builder factory was not configured.");
		}

		public void UseBuilder(Func<TestContextBuilder> builderFactory)
		{
			_builderFactory = builderFactory;
		}

		public void AddConfigurator(TestContextBuilderConfigurator configurator)
		{
			_configurators.Add(configurator);
		}

		public ITestContext Build()
		{
			TestContextBuilder builder = _builderFactory();

			builder = _configurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

			return builder.Build();
		}
	}
}