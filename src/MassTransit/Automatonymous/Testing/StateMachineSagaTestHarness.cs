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
namespace Automatonymous.Testing
{
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Testing;


    public class StateMachineSagaTestHarness<TInstance, TStateMachine> :
        SagaTestHarness<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TStateMachine :  SagaStateMachine<TInstance>
    {
        readonly TStateMachine _stateMachine;

        public StateMachineSagaTestHarness(BusTestHarness testHarness, ISagaRepository<TInstance> repository, TStateMachine stateMachine, string queueName)
            : base(testHarness, repository, queueName)
        {
            _stateMachine = stateMachine;
        }

        protected override void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_stateMachine, TestRepository);
        }

        protected override void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
        {
            configurator.ReceiveEndpoint(queueName, x =>
            {
                x.StateMachineSaga(_stateMachine, TestRepository);
            });
        }
    }
}