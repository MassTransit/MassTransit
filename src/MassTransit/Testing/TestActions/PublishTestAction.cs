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
namespace MassTransit.Testing.TestActions
{
	using System;
	using Scenarios;

	public class PublishTestAction<TScenario, TMessage> :
		TestAction<TScenario>
		where TMessage : class
		where TScenario : TestScenario
	{
		readonly Func<TScenario, IServiceBus> _busAccessor;
		readonly Action<TScenario, IPublishContext<TMessage>> _callback;
		readonly TMessage _message;

		public PublishTestAction(Func<TScenario, IServiceBus> busAccessor, TMessage message,
		                         Action<TScenario, IPublishContext<TMessage>> callback)
		{
			_message = message;
			_busAccessor = busAccessor;
			_callback = callback ?? DefaultCallback;
		}

		public void Act(TScenario scenario)
		{
			IServiceBus bus = _busAccessor(scenario);

			// give the message subscription time to show up on the InputBus, but don't require
			// it since it might be a negative test
			scenario.InputBus.HasSubscription<TMessage>();

			bus.Publish(_message, context => _callback(scenario, context));
		}

		static void DefaultCallback(TScenario scenario, IPublishContext<TMessage> context)
		{
		}
	}
}