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
namespace Automatonymous.RepositoryBuilders
{
    using System;
    using System.Linq.Expressions;
    using Internals.Caching;
    using MassTransit.Saga;


    class StateMachineSagaRepositoryBuilderImpl<TInstance> :
        StateMachineSagaRepositoryBuilder<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly Cache<Event, StateMachineEventCorrelation<TInstance>> _correlations;
        readonly ISagaRepository<TInstance> _repository;
        readonly StateMachine<TInstance> _stateMachine;
        Expression<Func<TInstance, bool>> _completedExpression;

        public StateMachineSagaRepositoryBuilderImpl(StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> repository, Cache<Event, StateMachineEventCorrelation<TInstance>> correlations)
        {
            _stateMachine = stateMachine;
            _repository = repository;
            _correlations = correlations;
            _completedExpression = x => false;
        }

        public StateMachine<TInstance> StateMachine
        {
            get { return _stateMachine; }
        }

        public void SetCompletedExpression(Expression<Func<TInstance, bool>> completedExpression)
        {
            _completedExpression = completedExpression;
        }

        public StateMachineSagaRepository<TInstance> Build()
        {
            return new AutomatonymousStateMachineSagaRepository<TInstance>(_repository, _completedExpression,
                _correlations);
        }
    }
}