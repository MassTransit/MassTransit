// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using MassTransit;
    using MassTransit.Logging;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using RepositoryBuilders;


    public class AutomatonymousStateMachineSagaRepository<TInstance> :
        StateMachineSagaRepository<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        static readonly ILog _log = Logger.Get<AutomatonymousStateMachineSagaRepository<TInstance>>();

        readonly Expression<Func<TInstance, bool>> _completedExpression;
        readonly Cache<Event, StateMachineEventCorrelation<TInstance>> _correlations;
        readonly Cache<Type, StateMachineEventCorrelation<TInstance>> _messageTypes;
        readonly ISagaRepository<TInstance> _repository;

        public AutomatonymousStateMachineSagaRepository(ISagaRepository<TInstance> repository,
            Expression<Func<TInstance, bool>> completedExpression,
            IEnumerable<StateMachineEventCorrelation<TInstance>> correlations)
        {
            _repository = repository;
            _completedExpression = completedExpression;

            StateMachineEventCorrelation<TInstance>[] eventCorrelations = correlations as StateMachineEventCorrelation<TInstance>[]
                ?? correlations.ToArray();

            _correlations = new DictionaryCache<Event, StateMachineEventCorrelation<TInstance>>(x => x.Event);
            _correlations.Fill(eventCorrelations);

            _messageTypes = new DictionaryCache<Type, StateMachineEventCorrelation<TInstance>>(
                x => x.Event.GetType().GetClosingArguments(typeof(Event<>)).Single());
            _messageTypes.Fill(eventCorrelations);
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context,
            Guid sagaId, InstanceHandlerSelector<TInstance, TMessage> selector, ISagaPolicy<TInstance, TMessage> policy)
            where TMessage : class
        {
            IEnumerable<Action<IConsumeContext<TMessage>>> handlers = _repository.GetSaga(context, sagaId, selector, policy);

            int handlerCount = 0;
            foreach (var handler in handlers)
            {
                handlerCount++;
                yield return handler;
            }

            int retryLimit;
            if (handlerCount == 0 && IsRetryEvent(context.Message, out retryLimit))
            {
                int attempts = context.RetryCount;
                if (attempts < retryLimit)
                {
                    yield return msgContext =>
                    {
                        _log.DebugFormat("Queuing {0} {1} for retry {2}", typeof(TMessage).Name, context.MessageId, attempts + 1);
                        msgContext.RetryLater();
                    };
                }
                else
                    _log.DebugFormat("Retry limit for {0} {1} reached {2}", typeof(TMessage).Name, context.MessageId, attempts + 1);
            }
        }

        public IEnumerable<Guid> Find(ISagaFilter<TInstance> filter)
        {
            return _repository.Find(filter);
        }

        public IEnumerable<TInstance> Where(ISagaFilter<TInstance> filter)
        {
            return _repository.Where(filter);
        }

        public IEnumerable<TResult> Where<TResult>(ISagaFilter<TInstance> filter, Func<TInstance, TResult> transformer)
        {
            return _repository.Where(filter, transformer);
        }

        public IEnumerable<TResult> Select<TResult>(Func<TInstance, TResult> transformer)
        {
            return _repository.Select(transformer);
        }

        public bool TryGetCorrelationExpressionForEvent<TData>(Event<TData> @event,
            out Expression<Func<TInstance, TData, bool>> correlationExpression,
            out Func<TData, Guid> correlationIdSelector) where TData : class
        {
            StateMachineEventCorrelation<TInstance> correlation;
            if (_correlations.TryGetValue(@event, out correlation))
            {
                correlationExpression = correlation.GetCorrelationExpression<TData>();
                correlationIdSelector = x => correlation.GetCorrelationId(x);
                return true;
            }

            correlationExpression = null;
            correlationIdSelector = null;
            return false;
        }

        public Expression<Func<TInstance, bool>> GetCompletedExpression()
        {
            return _completedExpression;
        }

        bool IsRetryEvent<TMessage>(TMessage message, out int retryLimit)
        {
            StateMachineEventCorrelation<TInstance> correlation;
            if (_messageTypes.TryGetValue(typeof(TMessage), out correlation))
            {
                retryLimit = correlation.RetryLimit;
                return retryLimit > 0;
            }

            retryLimit = 0;
            return false;
        }
    }
}