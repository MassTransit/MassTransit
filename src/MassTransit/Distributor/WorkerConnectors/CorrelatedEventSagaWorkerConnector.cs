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
namespace MassTransit.Distributor.WorkerConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Magnum.StateMachine;
    using Saga;
    using Saga.Configuration;
    using Saga.Pipeline;

    public class CorrelatedEventSagaWorkerConnector<TSaga, TMessage> :
        SagaWorkerConnectorBase<TSaga, TMessage>
        where TMessage : class, CorrelatedBy<Guid>
        where TSaga : SagaStateMachine<TSaga>, ISaga
    {
        readonly DataEvent<TSaga, TMessage> _dataEvent;
        readonly ISagaPolicyFactory _policyFactory;
        readonly Expression<Func<TSaga, bool>> _removeExpression;
        readonly IEnumerable<State> _states;

        public CorrelatedEventSagaWorkerConnector(ISagaRepository<TSaga> sagaRepository,
            DataEvent<TSaga, TMessage> dataEvent,
            IEnumerable<State> states,
            ISagaPolicyFactory policyFactory,
            Expression<Func<TSaga, bool>> removeExpression)
            : base(sagaRepository)
        {
            _dataEvent = dataEvent;
            _states = states;
            _policyFactory = policyFactory;
            _removeExpression = removeExpression;
        }

        protected override ISagaPolicy<TSaga, TMessage> GetPolicy()
        {
            return _policyFactory.GetPolicy<TSaga, TMessage>(_states, x => x.CorrelationId, _removeExpression);
        }

        protected override ISagaMessageSink<TSaga, TMessage> GetSagaMessageSink(ISagaRepository<TSaga> sagaRepository,
            ISagaPolicy<TSaga, TMessage> policy)
        {
            return new CorrelatedSagaStateMachineMessageSink<TSaga, TMessage>(sagaRepository, policy, _dataEvent);
        }
    }
}