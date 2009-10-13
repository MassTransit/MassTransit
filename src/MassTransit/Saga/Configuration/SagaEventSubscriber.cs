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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Exceptions;
	using Magnum.Reflection;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using Pipeline;

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

		public UnsubscribeAction Connect<V>(DataEvent<TSaga, V> eevent, IEnumerable<State> states)
			where V : class
		{
			Type messageType = typeof (V);

			ISagaPolicy<TSaga, V> policy = _policyFactory.GetPolicy<TSaga, V>(states);

			Expression<Func<TSaga, V, bool>> expression;
			if (SagaStateMachine<TSaga>.TryGetCorrelationExpressionForEvent(eevent, out expression))
			{
				return this.Call<UnsubscribeAction>("ConnectSink", eevent, policy, expression);
			}

			// we check for a standard correlation interface second
			if (messageType.GetInterfaces().Where(x => x == typeof (CorrelatedBy<Guid>)).Count() > 0)
			{
				return this.Call<UnsubscribeAction>("ConnectCorrelatedSink", eevent, policy);
			}

			throw new NotSupportedException("No method to connect to event was found for " + typeof (V).FullName);
		}

		protected virtual UnsubscribeAction ConnectCorrelatedSink<V>(DataEvent<TSaga, V> dataEvent, ISagaPolicy<TSaga, V> policy)
			where V : class, CorrelatedBy<Guid>
		{
			var repository = _context.Builder.GetInstance<ISagaRepository<TSaga>>();

			var sink = new CorrelatedSagaStateMachineMessageSink<TSaga, V>(_context, _context.Data as IServiceBus, repository, policy, dataEvent);
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " + typeof (CorrelatedSagaStateMachineMessageSink<TSaga, V>).ToFriendlyName());

			return ConnectToRouter<V>(sink);
		}

		protected virtual UnsubscribeAction ConnectSink<V>(DataEvent<TSaga, V> dataEvent, ISagaPolicy<TSaga, V> policy, Expression<Func<TSaga, V, bool>> selector)
			where V : class
		{
			var repository = _context.Builder.GetInstance<ISagaRepository<TSaga>>();

			var sink = new PropertySagaStateMachineMessageSink<TSaga, V>(_context, _context.Data as IServiceBus, repository, policy, selector, dataEvent);
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " + typeof (PropertySagaStateMachineMessageSink<TSaga, V>).ToFriendlyName());

			return ConnectToRouter<V>(sink);
		}

		protected virtual UnsubscribeAction ConnectToRouter<V>(IPipelineSink<V> sink)
			where V : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(_context.Pipeline);

			var router = routerConfigurator.FindOrCreate<V>();

			var result = router.Connect(sink);

			UnsubscribeAction remove = _context.SubscribedTo<V>();

			return () => result() && (router.SinkCount == 0) && remove();
		}
	}
}