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
	using System.Collections.Generic;
	using Instances;
	using Scenarios;
	using TestActions;

	public class HandlerTestBuilderImpl<TMessage> :
		HandlerTestBuilder<TMessage>
		where TMessage : class
	{
		readonly BusTestScenario _testContext;
		IList<TestAction> _actions;
		Action<IServiceBus, TMessage> _handler;


		public HandlerTestBuilderImpl(BusTestScenario testContext)
		{
			_testContext = testContext;
			_handler = DefaultHandler;

			_actions = new List<TestAction>();
		}

		public HandlerTest<TMessage> Build()
		{
			var test = new HandlerTestInstance<TMessage>(_testContext, _actions, _handler);

			return test;
		}

		public void SetHandler(Action<IServiceBus, TMessage> handler)
		{
			_handler = handler;
		}

		public void AddTestAction(TestAction testAction)
		{
			_actions.Add(testAction);
		}

		static void DefaultHandler(IServiceBus bus, TMessage message)
		{
		}
	}
}