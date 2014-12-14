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
namespace MassTransit.Testing
{
    using System;
    using Automatonymous;
    using Automatonymous.RepositoryConfigurators;
    using Automatonymous.Testing;
    using Scenarios;
    using TestInstanceConfigurators;


    public static class StateMachineSagaTestingExtensions
    {
        public static void UseStateMachineBuilder<TScenario, TSaga, TStateMachine>(
            this SagaTestInstanceConfigurator<TScenario, TSaga> configurator, TStateMachine stateMachine)
            where TSaga : class, SagaStateMachineInstance
            where TScenario : TestScenario
            where TStateMachine : StateMachine<TSaga>
        {
            configurator.UseBuilder(scenario =>
                                    new StateMachineSagaTestBuilderImpl<TScenario, TSaga, TStateMachine>(scenario,
                                        stateMachine, x => { }));
        }

        public static void UseStateMachineBuilder<TScenario, TSaga, TStateMachine>(
            this SagaTestInstanceConfigurator<TScenario, TSaga> configurator, TStateMachine stateMachine,
            Action<StateMachineSagaRepositoryConfigurator<TSaga>> configureCallback)
            where TSaga : class, SagaStateMachineInstance
            where TScenario : TestScenario
            where TStateMachine : StateMachine<TSaga>
        {
            configurator.UseBuilder(scenario =>
                                    new StateMachineSagaTestBuilderImpl<TScenario, TSaga, TStateMachine>(scenario,
                                        stateMachine, configureCallback));
        }

        public static TSaga ContainsInState<TSaga>(this SagaList<TSaga> sagas, Guid sagaId,
            State state, StateMachine<TSaga> machine)
            where TSaga : class, SagaStateMachineInstance
        {
            bool any = sagas.Any(x => x.CorrelationId == sagaId && machine.InstanceStateAccessor.Get(x).Equals(state));
            return any ? sagas.Contains(sagaId) : null;
        }
    }
}