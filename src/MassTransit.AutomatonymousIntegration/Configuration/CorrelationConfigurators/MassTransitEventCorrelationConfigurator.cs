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
    using MassTransit.Saga;


    public class MassTransitEventCorrelationConfigurator<TInstance, TData> :
        EventCorrelationConfigurator<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _machine;
        Func<ConsumeContext<TData>, Guid> _initialSagaIdSelector;
        Func<ISagaRepository<TInstance>, ISagaPolicy<TInstance, TData>, ISagaLocator<TData>> _locatorFactory;

        public MassTransitEventCorrelationConfigurator(SagaStateMachine<TInstance> machine, Event<TData> @event)
        {
            _event = @event;
            _machine = machine;
        }

        public EventCorrelationConfigurator<TInstance, TData> CorrelateById(Func<ConsumeContext<TData>, Guid> selector)
        {
            _locatorFactory = (_, __) => new CorrelationIdSagaLocator<TData>(selector);

            _initialSagaIdSelector = selector;

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

            var filterFactory = new PropertyExpressionSagaFilterFactory<TInstance, TData, T?>(propertyExpression, selector);

            _locatorFactory = (repository, policy) => new EventCorrelationSagaLocator<TInstance, TData>(repository, filterFactory, policy);

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

            var filterFactory = new PropertyExpressionSagaFilterFactory<TInstance, TData, T>(propertyExpression, selector);

            _locatorFactory = (repository, policy) => new EventCorrelationSagaLocator<TInstance, TData>(repository, filterFactory, policy);

            return this;
        }

        public EventCorrelationConfigurator<TInstance, TData> SelectId(Func<ConsumeContext<TData>, Guid> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            _initialSagaIdSelector = selector;

            return this;
        }

        public EventCorrelationConfigurator<TInstance, TData> CorrelateBy(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression)
        {
            if (correlationExpression == null)
                throw new ArgumentNullException("correlationExpression");

            var filterFactory = new ExpressionCorrelationSagaFilterFactory<TInstance, TData>(correlationExpression);

            _locatorFactory = (repository, policy) => new EventCorrelationSagaLocator<TInstance, TData>(repository, filterFactory, policy);

            return this;
        }

        public EventCorrelation<TInstance> Build()
        {
            return new MassTransitEventCorrelation<TInstance, TData>(_event, _initialSagaIdSelector, _machine, _locatorFactory);
        }
    }
}