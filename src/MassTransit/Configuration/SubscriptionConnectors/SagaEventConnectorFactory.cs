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
namespace MassTransit.SubscriptionConnectors
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Magnum.StateMachine;
	using Saga;
	using Saga.Configuration;

	public interface SagaEventConnectorFactory
	{
		IEnumerable<SagaSubscriptionConnector> Create();
	}

	public class SagaEventConnectorFactory<TSaga, TMessage> :
		SagaEventConnectorFactory
		where TSaga : SagaStateMachine<TSaga>, ISaga
		where TMessage : class
	{
		readonly DataEvent<TSaga, TMessage> _dataEvent;
		readonly ISagaRepository<TSaga> _sagaRepository;
		ISagaPolicy<TSaga, TMessage> _policy;
		ISagaPolicyFactory _policyFactory;

		public SagaEventConnectorFactory(ISagaRepository<TSaga> sagaRepository, ISagaPolicyFactory policyFactory,
		                                 DataEvent<TSaga, TMessage> dataEvent, IEnumerable<State> states)
		{
			_sagaRepository = sagaRepository;
			_policyFactory = policyFactory;
			_dataEvent = dataEvent;

			Expression<Func<TSaga, bool>> removeExpression = SagaStateMachine<TSaga>.GetCompletedExpression();

			_policy = _policyFactory.GetPolicy<TSaga, TMessage>(states, removeExpression);
		}

		public IEnumerable<SagaSubscriptionConnector> Create()
		{
			Expression<Func<TSaga, TMessage, bool>> expression;
			if (SagaStateMachine<TSaga>.TryGetCorrelationExpressionForEvent(_dataEvent, out expression))
			{
				yield return (SagaSubscriptionConnector) FastActivator.Create(typeof (PropertySagaSubscriptionConnector<,>),
					new[] {typeof (TSaga), typeof (TMessage)},
					new object[] {_sagaRepository, _dataEvent, _policy, expression});
			}
			else if (typeof (TMessage).Implements<CorrelatedBy<Guid>>())
			{
				yield return (SagaSubscriptionConnector) FastActivator.Create(typeof (CorrelatedSagaSubscriptionConnector<,>),
					new[] {typeof (TSaga), typeof (TMessage)},
					new object[] {_sagaRepository, _dataEvent, _policy});
			}
			else
				throw new NotSupportedException("No method to connect to event was found for " + typeof (TMessage).FullName);
		}
	}
}