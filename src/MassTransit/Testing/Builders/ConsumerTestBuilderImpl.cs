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
	using System.Collections.Generic;
	using Instances;
	using Scenarios;
	using TestActions;

	public class ConsumerTestBuilderImpl<TScenario, TConsumer> :
		ConsumerTestBuilder<TScenario, TConsumer>
		where TConsumer : class, IConsumer
	    where TScenario : IBusEndpointTestScenario
	{
		readonly IList<ITestAction<TScenario>> _actions;
		readonly TScenario _scenario;
		IConsumerFactory<TConsumer> _consumerFactory;
		public ConsumerTestBuilderImpl(TScenario scenario)
		{
			_scenario = scenario;

			_actions = new List<ITestAction<TScenario>>();
		}

		public ConsumerTest<TScenario, TConsumer> Build()
		{
			var test = new ConsumerTestInstance<TScenario, TConsumer>(_scenario, _actions, _consumerFactory);

			return test;
		}

		public void SetConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
		{
			_consumerFactory = consumerFactory;
		}

		public void AddTestAction(ITestAction<TScenario> testAction)
		{
			_actions.Add(testAction);
		}
	}
}