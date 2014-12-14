// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
namespace Automatonymous.RepositoryBuilders
{
    using System;
    using System.Linq.Expressions;
    using Internals.Extensions;
    using MassTransit;
    using MassTransit.Saga.Pipeline;
    using RepositoryConfigurators;


    public class StateMachineEventCorrelationImpl<TInstance, TData> :
        StateMachineEventCorrelationConfigurator<TInstance, TData>,
        StateMachineEventCorrelation<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, TData, bool>> _correlationExpression;
        readonly Event<TData> _event;
        Func<TData, Guid> _correlationIdSelector;
        int _retryLimit;

        public StateMachineEventCorrelationImpl(Event<TData> @event,
            Expression<Func<TInstance, TData, bool>> correlationExpression)
        {
            _correlationExpression = correlationExpression;
            _correlationIdSelector = GenerateCorrelationIdSelector(correlationExpression);
            _event = @event;
        }

        public Event Event
        {
            get { return _event; }
        }

        public int RetryLimit
        {
            get { return _retryLimit; }
        }

        public Expression<Func<TInstance, TMessage, bool>> GetCorrelationExpression<TMessage>()
            where TMessage : class
        {
            var self = this as StateMachineEventCorrelationImpl<TInstance, TMessage>;
            if (self == null)
            {
                throw new ArgumentException("The correlation is for messages of type " + typeof(TData).GetTypeName() +
                                            " but the method type is " + typeof(TMessage).GetTypeName());
            }

            return self._correlationExpression;
        }

        public Guid GetCorrelationId<TMessage>(TMessage message)
            where TMessage : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var self = this as StateMachineEventCorrelationImpl<TInstance, TMessage>;
            if (self == null)
            {
                throw new ArgumentException("The correlation is for messages of type " + typeof(TData).GetTypeName() +
                                            " but the method type is " + typeof(TMessage).GetTypeName());
            }

            return self._correlationIdSelector(message);
        }

        public StateMachineEventCorrelationConfigurator<TInstance, TData> SelectCorrelationId(
            Func<TData, Guid> correlationIdSelector)
        {
            if (correlationIdSelector == null)
                throw new ArgumentNullException("correlationIdSelector");

            _correlationIdSelector = correlationIdSelector;

            return this;
        }

        StateMachineEventCorrelationConfigurator<TInstance, TData> StateMachineEventCorrelationConfigurator<TInstance, TData>.RetryLimit(
            int retryLimit)
        {
            _retryLimit = retryLimit;
            return this;
        }

        static Func<TData, Guid> GenerateCorrelationIdSelector(Expression<Func<TInstance, TData, bool>> selector)
        {
            var visitor = new CorrelationExpressionToSagaIdVisitor<TInstance, TData>();

            Expression<Func<TData, Guid>> exp = visitor.Build(selector);

            return exp != null ? exp.Compile() : (x => NewId.NextGuid());
        }
    }
}