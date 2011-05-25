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
namespace MassTransit.Testing.Builders
{
	using System;
	using ContextBuilders;
	using Contexts;

	public class HandlerTestBuilderImpl<TMessage> :
		HandlerTestBuilder<TMessage>
		where TMessage : class
	{
		readonly ITestContext _testContext;
		Func<TestContextBuilder> _contextBuilderFactory;

		public HandlerTestBuilderImpl(ITestContext testContext)
		{
			_testContext = testContext;
		}

		public HandlerTest<TMessage> Build()
		{
			throw new NotImplementedException();
		}

		public void UseContextBuilder(Func<TestContextBuilder> builderFactory)
		{
			_contextBuilderFactory = builderFactory;
		}

		public void AddContextConfigurator(TestContextBuilderConfigurator configurator)
		{
			throw new NotImplementedException();
		}
	}
}