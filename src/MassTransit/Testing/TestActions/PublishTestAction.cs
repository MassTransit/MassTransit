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
	using Configurators;

	public class PublishTestAction<TMessage> :
		TestAction
		where TMessage : class
	{
		readonly Action<IPublishContext<TMessage>> _callback;
		readonly TMessage _message;

		public PublishTestAction(TMessage message, Action<IPublishContext<TMessage>> callback)
		{
			_message = message;
			_callback = callback ?? DefaultCallback;
		}

		public void Act(IServiceBus bus)
		{
			bus.Publish(_message, _callback);
		}

		static void DefaultCallback(IPublishContext<TMessage> context)
		{
		}
	}
}