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
namespace Automatonymous.Testing
{
    using System.Collections.Generic;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using MassTransit.Testing.Builders;
    using MassTransit.Testing.TestActions;


    public class StateMachineSagaTestBuilderImpl<TScenario, TSaga, TStateMachine> :
        SagaTestBuilder<TScenario, TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TScenario : ITestScenario
        where TStateMachine : SagaStateMachine<TSaga>
    {
        readonly IList<ITestAction<TScenario>> _actions;
        readonly TScenario _scenario;
        readonly TStateMachine _stateMachine;
        ISagaRepository<TSaga> _sagaRepository;

        public StateMachineSagaTestBuilderImpl(TScenario scenario, TStateMachine stateMachine)
        {
            _scenario = scenario;
            _stateMachine = stateMachine;

            _actions = new List<ITestAction<TScenario>>();
        }

        public SagaTest<TScenario, TSaga> Build()
        {
            if (_sagaRepository == null)
                _sagaRepository = new InMemorySagaRepository<TSaga>();

            var test = new StateMachineSagaTestInstance<TScenario, TSaga, TStateMachine>(_scenario, _actions, _sagaRepository, _stateMachine);

            return test;
        }

        public void SetSagaRepository(ISagaRepository<TSaga> sagaRepository)
        {
            _sagaRepository = sagaRepository;
        }

        public void AddTestAction(ITestAction<TScenario> testAction)
        {
            _actions.Add(testAction);
        }
    }
}