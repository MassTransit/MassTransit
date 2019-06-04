// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Saga.Factories;
    using MassTransit.Saga.Pipeline.Filters;
    using Saga.QueryFactories;


    public class MassTransitEventCorrelationConfigurator<TInstance, TData> :
        IEventCorrelationConfigurator<TInstance, TData>,
        IEventCorrelationBuilder
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _machine;
        bool _insertOnInitial;
        IFilter<ConsumeContext<TData>> _messageFilter;
        IPipe<ConsumeContext<TData>> _missingPipe;
        ISagaFactory<TInstance, TData> _sagaFactory;
        SagaFilterFactory<TInstance, TData> _sagaFilterFactory;

        public MassTransitEventCorrelationConfigurator(SagaStateMachine<TInstance> machine, Event<TData> @event, EventCorrelation existingCorrelation)
        {
            _event = @event;
            _machine = machine;
            _insertOnInitial = false;

            _sagaFactory = new DefaultSagaFactory<TInstance, TData>();

            var correlation = existingCorrelation as EventCorrelation<TInstance, TData>;
            if (correlation != null)
            {
                _sagaFilterFactory = correlation.FilterFactory;
                _messageFilter = correlation.MessageFilter;
            }
        }

        public EventCorrelation Build()
        {
            return new MassTransitEventCorrelation<TInstance, TData>(_machine, _event, _sagaFilterFactory, _messageFilter, _missingPipe, _sagaFactory,
                _insertOnInitial);
        }

        public bool InsertOnInitial
        {
            set { _insertOnInitial = value; }
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateById(Func<ConsumeContext<TData>, Guid> selector)
        {
            _messageFilter = new CorrelationIdMessageFilter<TData>(selector);

            _sagaFilterFactory = (repository, policy, sagaPipe) => new CorrelatedSagaFilter<TInstance, TData>(repository, policy, sagaPipe);

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateById<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : struct
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T>(propertyExpression, selector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T?>> propertyExpression,
            Func<ConsumeContext<TData>, T?> selector)
            where T : struct
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T?>(propertyExpression, selector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : class
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T>(propertyExpression, selector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> SelectId(Func<ConsumeContext<TData>, Guid> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _messageFilter = new CorrelationIdMessageFilter<TData>(selector);

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> CorrelateBy(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression)
        {
            if (correlationExpression == null)
                throw new ArgumentNullException(nameof(correlationExpression));

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new ExpressionCorrelationSagaQueryFactory<TInstance, TData>(correlationExpression);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> SetSagaFactory(SagaFactoryMethod<TInstance, TData> factoryMethod)
        {
            _sagaFactory = new FactoryMethodSagaFactory<TInstance, TData>(factoryMethod);

            return this;
        }

        public IEventCorrelationConfigurator<TInstance, TData> OnMissingInstance(
            Func<IMissingInstanceConfigurator<TInstance, TData>, IPipe<ConsumeContext<TData>>> getMissingPipe)
        {
            if (getMissingPipe == null)
                throw new ArgumentNullException(nameof(getMissingPipe));

            var configurator = new EventMissingInstanceConfigurator<TInstance, TData>();

            _missingPipe = getMissingPipe(configurator);

            return this;
        }
    }
}
