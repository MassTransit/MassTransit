// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Saga;


    public static class StateMachineSagaTestHarnessExtensions
    {
        public static StateMachineSagaTestHarness<TInstance, TStateMachine> StateMachineSaga<TInstance, TStateMachine>(this BusTestHarness harness,
            TStateMachine stateMachine, string queueName = null)
            where TInstance : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TInstance>
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));
            var repository = new InMemorySagaRepository<TInstance>();

            return new StateMachineSagaTestHarness<TInstance, TStateMachine>(harness, repository, stateMachine, queueName);
        }

        public static StateMachineSagaTestHarness<TInstance, TStateMachine> StateMachineSaga<TInstance, TStateMachine>(this BusTestHarness harness,
            TStateMachine stateMachine, ISagaRepository<TInstance> repository, string queueName = null)
            where TInstance : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TInstance>
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            return new StateMachineSagaTestHarness<TInstance, TStateMachine>(harness, repository, stateMachine, queueName);
        }

        public static TSaga ContainsInState<TSaga>(this ISagaList<TSaga> sagas, Guid sagaId,
            State state, SagaStateMachine<TSaga> machine)
            where TSaga : class, SagaStateMachineInstance
        {
            bool any = sagas.Select(x => x.CorrelationId == sagaId && machine.Accessor.GetState(x).Result.Equals(state)).Any();
            return any ? sagas.Contains(sagaId) : null;
        }
    }
}