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
namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Internals.Extensions;
    using MassTransit.Monitoring.Introspection;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Util;
    using RepositoryBuilders;


    public class AutomatonymousStateMachineSagaRepository<TInstance> :
        IStateMachineSagaRepository<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly Expression<Func<TInstance, bool>> _completedExpression;
        readonly Dictionary<Event, StateMachineEventCorrelation<TInstance>> _correlations;
        readonly Dictionary<Type, StateMachineEventCorrelation<TInstance>> _messageTypes;
        readonly ISagaRepository<TInstance> _repository;

        public AutomatonymousStateMachineSagaRepository(ISagaRepository<TInstance> repository,
            Expression<Func<TInstance, bool>> completedExpression,
            IEnumerable<StateMachineEventCorrelation<TInstance>> correlations)
        {
            _repository = repository;
            _completedExpression = completedExpression;

            StateMachineEventCorrelation<TInstance>[] eventCorrelations = correlations.ToArray();

            _correlations = new Dictionary<Event, StateMachineEventCorrelation<TInstance>>();
            foreach (var correlation in eventCorrelations)
                _correlations.Add(correlation.Event, correlation);

            _messageTypes = new Dictionary<Type, StateMachineEventCorrelation<TInstance>>();
            foreach (var eventCorrelation in eventCorrelations)
                _messageTypes.Add(eventCorrelation.Event.GetType().GetClosingArguments(typeof(Event<>)).Single(), eventCorrelation);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("sagaRepository");
            scope.Set(new
            {
                InstanceType = TypeMetadataCache<TInstance>.ShortName,
                Events = _messageTypes.ToDictionary(x => x.Value.Event.Name, x => new
                {
                    DataType = TypeMetadataCache.GetShortName(x.Key)
                })
            });

            _repository.Probe(scope);
        }

        public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TInstance, T> policy, IPipe<SagaConsumeContext<TInstance, T>> next) where T : class
        {
            return _repository.Send(context, policy, next);
        }

        public Task SendQuery<T>(SagaQueryConsumeContext<TInstance, T> context, ISagaPolicy<TInstance, T> policy, IPipe<SagaConsumeContext<TInstance, T>> next)
            where T : class
        {
            return _repository.SendQuery(context, policy, next);
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
    }
}