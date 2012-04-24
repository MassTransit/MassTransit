// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.DistributorConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Magnum.StateMachine;
    using Saga;
    using Saga.Configuration;

    public interface EventDistributorConnectorFactory
    {
        IEnumerable<SagaDistributorConnector> Create();
    }

    public class EventDistributorConnectorFactory<TSaga, TMessage> :
        EventDistributorConnectorFactory
        where TSaga : SagaStateMachine<TSaga>, ISaga
        where TMessage : class
    {
        readonly DataEvent<TSaga, TMessage> _dataEvent;
        readonly ISagaPolicyFactory _policyFactory;
        readonly Expression<Func<TSaga, bool>> _removeExpression;
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly IEnumerable<State> _states;
        readonly IWorkerSelectorFactory _workerSelectorFactory;

        public EventDistributorConnectorFactory(IWorkerSelectorFactory workerSelectorFactory,
            ISagaRepository<TSaga> sagaRepository, ISagaPolicyFactory policyFactory,
            DataEvent<TSaga, TMessage> dataEvent, IEnumerable<State> states)
        {
            _workerSelectorFactory = workerSelectorFactory;
            _sagaRepository = sagaRepository;
            _policyFactory = policyFactory;
            _dataEvent = dataEvent;
            _states = states;

            _removeExpression = SagaStateMachine<TSaga>.GetCompletedExpression();
        }

        public IEnumerable<SagaDistributorConnector> Create()
        {
            Expression<Func<TSaga, TMessage, bool>> expression;
            if (SagaStateMachine<TSaga>.TryGetCorrelationExpressionForEvent(_dataEvent, out expression))
            {
                yield return
                    (SagaDistributorConnector)FastActivator.Create(typeof(PropertyEventSagaDistributorConnector<,>),
                        new[] {typeof(TSaga), typeof(TMessage)},
                        new object[]
                            {
                                _workerSelectorFactory, _sagaRepository, _dataEvent, _states, _policyFactory,
                                _removeExpression, expression
                            });
            }
            else if (typeof(TMessage).Implements<CorrelatedBy<Guid>>())
            {
                yield return
                    (SagaDistributorConnector)FastActivator.Create(typeof(CorrelatedEventSagaDistributorConnector<,>),
                        new[] {typeof(TSaga), typeof(TMessage)},
                        new object[]
                            {
                                _workerSelectorFactory, _sagaRepository, _dataEvent, _states, _policyFactory,
                                _removeExpression
                            });
            }
            else
                throw new NotSupportedException("No method to connect to event was found for "
                                                + typeof(TMessage).FullName);
        }
    }
}