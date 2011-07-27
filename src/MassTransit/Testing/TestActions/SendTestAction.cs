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

	public class SendTestAction<TScenario, TMessage> :
		TestAction<TScenario>
		where TMessage : class
		where TScenario : TestScenario
	{
		readonly Action<TScenario, ISendContext<TMessage>> _callback;
		readonly Func<TScenario, IEndpoint> _endpointAccessor;
		readonly TMessage _message;

		public SendTestAction(Func<TScenario, IEndpoint> endpointAccessor, TMessage message,
		                      Action<TScenario, ISendContext<TMessage>> callback)
		{
			_message = message;
			_endpointAccessor = endpointAccessor;
			_callback = callback ?? DefaultCallback;
		}

		public void Act(TScenario scenario)
		{
			IEndpoint endpoint = _endpointAccessor(scenario);

			endpoint.Send(_message, context => _callback(scenario, context));
		}

		static void DefaultCallback(TScenario scenario, ISendContext<TMessage> context)
		{
		}
	}
}