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
    using Magnum.Extensions;
    using Magnum.StateMachine;
    using Saga;
    using Saga.Configuration;
    using Saga.Pipeline;

    public class PropertyEventSagaWorkerConnector<TSaga, TMessage> :
        SagaWorkerConnectorBase<TSaga, TMessage>
        where TMessage : class
        where TSaga : SagaStateMachine<TSaga>, ISaga
    {
        readonly DataEvent<TSaga, TMessage> _dataEvent;
        readonly ISagaPolicyFactory _policyFactory;
        readonly Expression<Func<TSaga, bool>> _removeExpression;
        readonly IEnumerable<State> _states;
        Func<TMessage, Guid> _correlationIdSelector;
        Expression<Func<TSaga, TMessage, bool>> _bindExpression;

        public PropertyEventSagaWorkerConnector(ISagaRepository<TSaga> sagaRepository,
            DataEvent<TSaga, TMessage> dataEvent,
            IEnumerable<State> states,
            ISagaPolicyFactory policyFactory,
            Expression<Func<TSaga, bool>> removeExpression,
            EventBinder<TSaga> eventBinder)
            : base(sagaRepository)
        {
            _dataEvent = dataEvent;
            _states = states;
            _policyFactory = policyFactory;
            _removeExpression = removeExpression;

            _bindExpression = eventBinder.GetBindExpression<TMessage>();

            _correlationIdSelector = GetCorrelationIdSelector(eventBinder);
        }

        protected override ISagaPolicy<TSaga, TMessage> GetPolicy()
        {
            return _policyFactory.GetPolicy(_states, _correlationIdSelector, _removeExpression);
        }

        protected override ISagaMessageSink<TSaga, TMessage> GetSagaMessageSink(ISagaRepository<TSaga> sagaRepository,
            ISagaPolicy<TSaga, TMessage> policy)
        {
            return new PropertySagaStateMachineMessageSink<TSaga, TMessage>(sagaRepository, policy,
                _bindExpression, _dataEvent);
        }

        static Func<TMessage, Guid> GetCorrelationIdSelector(EventBinder<TSaga> binder)
        {
            Func<TMessage, Guid> correlationIdSelector = binder.GetCorrelationIdSelector<TMessage>();
            if (correlationIdSelector != null)
                return correlationIdSelector;

            var visitor = new CorrelationExpressionToSagaIdVisitor<TSaga, TMessage>();

            Expression<Func<TMessage, Guid>> exp = visitor.Build(binder.GetBindExpression<TMessage>());
            if (exp != null)
                return exp.Compile();

            if (typeof(TMessage).Implements<CorrelatedBy<Guid>>())
            {
                Type genericType = typeof(Correlated<>).MakeGenericType(typeof(TSaga), typeof(TMessage), typeof(TMessage));

                var correlated = (ICorrelated<TMessage>)Activator.CreateInstance(genericType);

                return correlated.CorrelationIdSelector;
            }

            return x => NewId.NextGuid();
        }

        class Correlated<T> :
            ICorrelated<T>
            where T : CorrelatedBy<Guid>
        {
            public Func<T, Guid> CorrelationIdSelector
            {
                get { return x => x.CorrelationId; }
            }
        }

        interface ICorrelated<T>
        {
            Func<T, Guid> CorrelationIdSelector { get; }
        }
    }
}