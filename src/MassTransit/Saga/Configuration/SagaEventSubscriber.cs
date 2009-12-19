// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Saga.Configuration
{
	using System;
	using System.Collections.Generic;
	using Magnum.Reflection;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;

	public class SagaEventSubscriber<TSaga>
		where TSaga : SagaStateMachine<TSaga>, ISaga
	{
		private readonly ISubscriberContext _context;
		private readonly ISagaPolicyFactory _policyFactory;

		public SagaEventSubscriber(ISubscriberContext context, ISagaPolicyFactory policyFactory)
		{
			_context = context;
			_policyFactory = policyFactory;
		}

		public UnsubscribeAction Connect(Type messageType, Event @event, IEnumerable<State> states)
		{
			return this.FastInvoke<SagaEventSubscriber<TSaga>, UnsubscribeAction>(new[] {messageType}, "ConnectTo", @event, states);
		}

		private UnsubscribeAction ConnectTo<TMessage>(DataEvent<TSaga, TMessage> eevent, IEnumerable<State> states)
			where TMessage : class
		{
			var factory = new SagaStateMachineMessageSinkFactory<TSaga, TMessage>(_context, _policyFactory);
			IPipelineSink<TMessage> sink = factory.Create(eevent, states);

			return _context.Pipeline.ConnectToRouter(sink, () => _context.SubscribedTo<TMessage>());
		}
	}
}