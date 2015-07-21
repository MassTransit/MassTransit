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
namespace MassTransit.Testing
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Testing;
    using TestInstanceConfigurators;


    public static class StateMachineSagaTestingExtensions
    {
        public static void UseStateMachineBuilder<TScenario, TSaga, TStateMachine>(
            this ISagaTestConfigurator<TScenario, TSaga> configurator, TStateMachine stateMachine)
            where TSaga : class, SagaStateMachineInstance
            where TScenario : ITestScenario
            where TStateMachine : SagaStateMachine<TSaga>
        {
            configurator.UseBuilder(scenario =>
                new StateMachineSagaTestBuilderImpl<TScenario, TSaga, TStateMachine>(scenario,
                    stateMachine));
        }
        public static TSaga ContainsInState<TSaga>(this ISagaList<TSaga> sagas, Guid sagaId,
            State state, SagaStateMachine<TSaga> machine)
            where TSaga : class, SagaStateMachineInstance
        {
            bool any = sagas.Select(x => x.CorrelationId == sagaId && machine.InstanceStateAccessor.GetState(x).Equals(state)).Any();
            return any ? sagas.Contains(sagaId) : null;
        }
    }
}