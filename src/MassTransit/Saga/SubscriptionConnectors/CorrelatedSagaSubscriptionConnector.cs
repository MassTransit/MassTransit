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
namespace MassTransit.Saga.SubscriptionConnectors
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Configuration;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using Pipeline;
	using Util;

	public class CorrelatedSagaSubscriptionConnector<TSaga, TMessage> :
		SagaSubscriptionConnector
		where TSaga : SagaStateMachine<TSaga>, ISaga
		where TMessage : class, CorrelatedBy<Guid>
	{
		readonly DataEvent<TSaga, TMessage> _dataEvent;
		readonly ISagaPolicy<TSaga, TMessage> _policy;
		readonly ISagaRepository<TSaga> _sagaRepository;

		public CorrelatedSagaSubscriptionConnector(ISagaRepository<TSaga> sagaRepository, DataEvent<TSaga, TMessage> dataEvent,
		                                           IEnumerable<State> states,
		                                           ISagaPolicyFactory policyFactory,
		                                           Expression<Func<TSaga, bool>> removeExpression)
		{
			_sagaRepository = sagaRepository;
			_dataEvent = dataEvent;

			_policy = policyFactory.GetPolicy<TSaga, TMessage>(states, x => x.CorrelationId, removeExpression);
		}

		public Type SagaType
		{
			get { return typeof (TSaga); }
		}

		public Type MessageType
		{
			get { return typeof (TMessage); }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
		{
			ISagaMessageSink<TSaga, TMessage> sink = CreateSink();

			return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
		}

		public ISagaMessageSink<TSaga, TMessage> CreateSink()
		{
			var sink = new CorrelatedSagaStateMachineMessageSink<TSaga, TMessage>(_sagaRepository, _policy,
				_dataEvent);
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " +
				                                 typeof (CorrelatedSagaStateMachineMessageSink<TSaga, TMessage>).ToFriendlyName());
			return sink;
		}
	}
}