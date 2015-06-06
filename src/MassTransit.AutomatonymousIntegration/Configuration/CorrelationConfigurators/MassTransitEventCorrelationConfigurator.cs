// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Saga.Pipeline.Filters;


    public class MassTransitEventCorrelationConfigurator<TInstance, TData> :
        EventCorrelationConfigurator<TInstance, TData>,
        EventCorrelationBuilder<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _machine;
        IFilter<ConsumeContext<TData>> _messageFilter;
        SagaFilterFactory<TInstance, TData> _sagaFilterFactory;

        public MassTransitEventCorrelationConfigurator(SagaStateMachine<TInstance> machine, Event<TData> @event)
        {
            _event = @event;
            _machine = machine;
        }

        public EventCorrelation<TInstance> Build()
        {
            return new MassTransitEventCorrelation<TInstance, TData>(_machine, _event, _sagaFilterFactory, _messageFilter);
        }

        public EventCorrelationConfigurator<TInstance, TData> CorrelateById(Func<ConsumeContext<TData>, Guid> selector)
        {
            _messageFilter = new CorrelationIdMessageFilter<TData>(selector);
            _sagaFilterFactory = (repository, policy, sagaPipe) => new CorrelatedSagaFilter<TInstance, TData>(repository, policy, sagaPipe);

            return this;
        }

        public EventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T?>> propertyExpression,
            Func<ConsumeContext<TData>, T?> selector)
            where T : struct
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");
            if (selector == null)
                throw new ArgumentNullException("selector");

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T?>(propertyExpression, selector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public EventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : class
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");
            if (selector == null)
                throw new ArgumentNullException("selector");

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new PropertyExpressionSagaQueryFactory<TInstance, TData, T>(propertyExpression, selector);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }

        public EventCorrelationConfigurator<TInstance, TData> SelectId(Func<ConsumeContext<TData>, Guid> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            _messageFilter = new CorrelationIdMessageFilter<TData>(selector);

            return this;
        }

        public EventCorrelationConfigurator<TInstance, TData> CorrelateBy(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression)
        {
            if (correlationExpression == null)
                throw new ArgumentNullException("correlationExpression");

            _sagaFilterFactory = (repository, policy, sagaPipe) =>
            {
                var queryFactory = new ExpressionCorrelationSagaQueryFactory<TInstance, TData>(correlationExpression);

                return new QuerySagaFilter<TInstance, TData>(repository, policy, queryFactory, sagaPipe);
            };

            return this;
        }
    }
}