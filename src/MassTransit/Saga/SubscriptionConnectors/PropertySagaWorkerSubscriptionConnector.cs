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
	using Distributor;
	using Distributor.Messages;
	using Distributor.Pipeline;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;

    public class PropertySagaWorkerSubscriptionConnector<TSaga, TMessage> :
		SagaWorkerSubscriptionConnector
		where TSaga : SagaStateMachine<TSaga>, ISaga
		where TMessage : class
	{
		readonly PropertySagaSubscriptionConnector<TSaga, TMessage> _connector;

		public PropertySagaWorkerSubscriptionConnector(ISagaRepository<TSaga> sagaRepository,
		                                               DataEvent<TSaga, TMessage> dataEvent,
		                                               IEnumerable<State> states,
		                                               ISagaPolicyFactory policyFactory,
		                                               Expression<Func<TSaga, bool>> removeExpression,
		                                               Expression<Func<TSaga, TMessage, bool>> selector)
		{
			_connector = new PropertySagaSubscriptionConnector<TSaga, TMessage>(sagaRepository, dataEvent, states, policyFactory,
				removeExpression, selector);
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance)
		{
			var worker = instance as ISagaWorker<TSaga>;
			ISagaMessageSink<TSaga, TMessage> sink = _connector.CreateSink();
			var workerSink = new SagaWorkerMessageSink<TSaga, TMessage>(worker, sink);

			return configurator.Pipeline.ConnectToRouter(workerSink, () => configurator.SubscribedTo<Distributed<TMessage>>());
		}

		public Type MessageType
		{
			get { return typeof (TMessage); }
		}

		public Type SagaType
		{
			get { return typeof (TSaga); }
		}
	}
}