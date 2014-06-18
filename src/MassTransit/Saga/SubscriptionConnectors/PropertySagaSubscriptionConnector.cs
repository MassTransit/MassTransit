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
namespace MassTransit.Saga.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Configuration;
    using Magnum.Extensions;
    using Magnum.StateMachine;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using Pipeline;
    using Util;

    public class PropertySagaSubscriptionConnector<TSaga, TMessage> :
        SagaSubscriptionConnector
        where TSaga : SagaStateMachine<TSaga>, ISaga
        where TMessage : class
    {
        readonly DataEvent<TSaga, TMessage> _dataEvent;
        readonly ISagaPolicy<TSaga, TMessage> _policy;
        readonly ISagaRepository<TSaga> _sagaRepository;
        Expression<Func<TSaga, TMessage, bool>> _bindExpression;

        public PropertySagaSubscriptionConnector(ISagaRepository<TSaga> sagaRepository,
            DataEvent<TSaga, TMessage> dataEvent,
            IEnumerable<State> states,
            ISagaPolicyFactory policyFactory,
            Expression<Func<TSaga, bool>> removeExpression,
            EventBinder<TSaga> eventBinder)
        {
            _sagaRepository = sagaRepository;
            _dataEvent = dataEvent;

            _bindExpression = eventBinder.GetBindExpression<TMessage>();

            Func<TMessage, Guid> correlationIdSelector = GetCorrelationIdSelector(eventBinder);

            _policy = policyFactory.GetPolicy(states, correlationIdSelector, removeExpression);
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
        {
            ISagaMessageSink<TSaga, TMessage> sink = CreateSink();

            return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
        }

        public ISagaMessageSink<TSaga, TMessage> CreateSink()
        {
            var sink = new PropertySagaStateMachineMessageSink<TSaga, TMessage>(_sagaRepository, _policy,
                _bindExpression, _dataEvent);

            if (sink == null)
                throw new ConfigurationException("Could not build the message sink: " +
                                                 typeof(PropertySagaStateMachineMessageSink<TSaga, TMessage>).
                                                     ToFriendlyName());
            return sink;
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