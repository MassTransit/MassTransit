// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous.RepositoryConfigurators
{
    using System;
    using System.Linq.Expressions;
    using BuilderConfigurators;
    using Internals.Caching;
    using MassTransit.Saga;
    using RepositoryBuilders;


    public class StateMachineSagaRepositoryConfiguratorImpl<TInstance> :
        StateMachineSagaRepositoryConfigurator<TInstance>,
        StateMachineSagaRepositoryBuilderConfigurator<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly Cache<Event, StateMachineEventCorrelation<TInstance>> _correlations;
        readonly ISagaRepository<TInstance> _repository;
        readonly StateMachine<TInstance> _stateMachine;
        Expression<Func<TInstance, bool>> _removeExpression;

        public StateMachineSagaRepositoryConfiguratorImpl(StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> repository)
        {
            _stateMachine = stateMachine;
            _repository = repository;
            _correlations = new DictionaryCache<Event, StateMachineEventCorrelation<TInstance>>();
            _removeExpression = x => false;
        }

        public StateMachine<TInstance> StateMachine
        {
            get { return _stateMachine; }
        }

        public void RemoveWhen(Expression<Func<TInstance, bool>> removeExpression)
        {
            _removeExpression = removeExpression;
        }

        public StateMachineEventCorrelationConfigurator<TInstance, TData> Correlate<TData>(Event<TData> @event,
            Expression<Func<TInstance, TData, bool>> correlationExpression)
            where TData : class
        {
            var correlation = new StateMachineEventCorrelationImpl<TInstance, TData>(@event, correlationExpression);

            _correlations[@event] = correlation;

            return correlation;
        }

        public StateMachineSagaRepository<TInstance> Configure()
        {
            var builder = new StateMachineSagaRepositoryBuilderImpl<TInstance>(_stateMachine, _repository, _correlations);

            builder.SetCompletedExpression(_removeExpression);

            return builder.Build();
        }
    }
}