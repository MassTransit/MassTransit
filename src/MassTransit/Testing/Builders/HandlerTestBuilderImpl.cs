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

	public class HandlerTestBuilderImpl<TScenario, TMessage> :
		HandlerTestBuilder<TScenario, TMessage>
		where TMessage : class
		where TScenario : TestScenario
	{
		readonly TScenario _scenario;
		readonly IList<TestAction<TScenario>> _actions;
		Action<IConsumeContext<TMessage>, TMessage> _handler;


		public HandlerTestBuilderImpl(TScenario scenario)
		{
			_scenario = scenario;
			_handler = DefaultHandler;

			_actions = new List<TestAction<TScenario>>();
		}

		public HandlerTest<TScenario, TMessage> Build()
		{
			var test = new HandlerTestInstance<TScenario, TMessage>(_scenario, _actions, _handler);

			return test;
		}

		public void SetHandler(Action<IConsumeContext<TMessage>, TMessage> handler)
		{
			_handler = handler;
		}

		public void AddTestAction(TestAction<TScenario> testAction)
		{
			_actions.Add(testAction);
		}

		static void DefaultHandler(IConsumeContext<TMessage> bus, TMessage message)
		{
		}
	}
}