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
    using Magnum.StateMachine;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using Pipeline;
    using Saga;
    using Saga.Configuration;
    using Saga.Pipeline;

    public class PropertyEventSagaDistributorConnector<TSaga, TMessage> :
        SagaDistributorConnector
        where TSaga : SagaStateMachine<TSaga>, ISaga
        where TMessage : class
    {
        readonly DataEvent<TSaga, TMessage> _dataEvent;
        readonly ISagaPolicy<TSaga, TMessage> _policy;
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly IWorkerSelectorFactory _workerSelectorFactory;
        Expression<Func<TSaga, TMessage, bool>> _bindExpression;

        public PropertyEventSagaDistributorConnector(IWorkerSelectorFactory workerSelectorFactory,
            ISagaRepository<TSaga> sagaRepository,
            DataEvent<TSaga, TMessage> dataEvent,
            IEnumerable<State> states,
            ISagaPolicyFactory policyFactory,
            Expression<Func<TSaga, bool>> removeExpression,
            EventBinder<TSaga> eventBinder)
        {
            _workerSelectorFactory = workerSelectorFactory;
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

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IDistributor distributor)
        {
            IWorkerAvailability<TMessage> workerAvailability = distributor.GetWorkerAvailability<TMessage>();

            // TODO we need to make a saga worker availability so that we can split saga load by correlation id
            IWorkerSelector<TMessage> workerSelector = _workerSelectorFactory.GetSelector<TMessage>();

            var sink = new DistributorMessageSink<TMessage>(workerAvailability, workerSelector);

            return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
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