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
	using System.Linq;
	using System.Linq.Expressions;
	using Exceptions;
	using Magnum.Reflection;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using Pipeline;

	public class SagaStateMachineMessageSinkFactory<TSaga, TMessage>
		where TSaga : SagaStateMachine<TSaga>, ISaga
		where TMessage : class
	{
		private readonly ISubscriberContext _context;
		private readonly ISagaPolicyFactory _policyFactory;
		private IServiceBus _bus;
		private ISagaRepository<TSaga> _repository;

		public SagaStateMachineMessageSinkFactory(ISubscriberContext context, ISagaPolicyFactory policyFactory)
		{
			_context = context;
			_policyFactory = policyFactory;
			_bus = _context.Data as IServiceBus;
			_repository = _context.Builder.GetInstance<ISagaRepository<TSaga>>();
		}

		public IPipelineSink<TMessage> Create(DataEvent<TSaga, TMessage> eevent, IEnumerable<State> states)
		{
			Type messageType = typeof (TMessage);

			Expression<Func<TSaga, bool>> removeExpression = SagaStateMachine<TSaga>.GetCompletedExpression();

			ISagaPolicy<TSaga, TMessage> policy = _policyFactory.GetPolicy<TSaga, TMessage>(states, removeExpression);

			Expression<Func<TSaga, TMessage, bool>> expression;
			if (SagaStateMachine<TSaga>.TryGetCorrelationExpressionForEvent(eevent, out expression))
			{
				return this.FastInvoke<SagaStateMachineMessageSinkFactory<TSaga, TMessage>, IPipelineSink<TMessage>>("CreateSink", eevent, policy, expression);
			}

			// we check for a standard correlation interface second
			if (messageType.GetInterfaces().Where(x => x == typeof (CorrelatedBy<Guid>)).Count() > 0)
			{
				return this.FastInvoke<SagaStateMachineMessageSinkFactory<TSaga, TMessage>, IPipelineSink<TMessage>>("CreateCorrelatedSink", eevent, policy);
			}

			throw new NotSupportedException("No method to connect to event was found for " + typeof (TMessage).FullName);
		}

		protected virtual IPipelineSink<TMessage> CreateSink<V>(DataEvent<TSaga, V> dataEvent, ISagaPolicy<TSaga, V> policy, Expression<Func<TSaga, V, bool>> selector)
			where V : class
		{
			var sink = new PropertySagaStateMachineMessageSink<TSaga, V>(_context, _bus, _repository, policy, selector, dataEvent);
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " + typeof (PropertySagaStateMachineMessageSink<TSaga, V>).ToFriendlyName());

			return (IPipelineSink<TMessage>) sink;
		}

		protected virtual IPipelineSink<TMessage> CreateCorrelatedSink<V>(DataEvent<TSaga, V> dataEvent, ISagaPolicy<TSaga, V> policy)
			where V : class, CorrelatedBy<Guid>
		{
			var sink = new CorrelatedSagaStateMachineMessageSink<TSaga, V>(_context, _bus, _repository, policy, dataEvent);
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " + typeof (CorrelatedSagaStateMachineMessageSink<TSaga, V>).ToFriendlyName());

			return (IPipelineSink<TMessage>) sink;
		}
	}
}