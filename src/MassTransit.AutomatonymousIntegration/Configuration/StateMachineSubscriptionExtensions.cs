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
    using MassTransit;
    using MassTransit.Policies;
    using MassTransit.Saga;
    using RepositoryConfigurators;
    using SubscriptionConfigurators;
    using SubscriptionConnectors;


    public static class StateMachineSubscriptionExtensions
    {
        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="sagaRepository">The saga repository for the instances</param>
        /// <returns></returns>
        public static StateMachineSubscriptionConfigurator<TInstance> StateMachineSaga<TInstance>(
            this IReceiveEndpointConfigurator configurator, StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> sagaRepository)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineSagaRepositoryConfigurator = new StateMachineSagaRepositoryConfiguratorImpl<TInstance>(stateMachine,
                sagaRepository);

            StateMachineSagaRepository<TInstance> repository = stateMachineSagaRepositoryConfigurator.Configure();

            var stateMachineConfigurator = new StateMachineSubscriptionConfiguratorImpl<TInstance>(stateMachine,
                repository);

            configurator.AddConfigurator(stateMachineConfigurator);

            return stateMachineConfigurator;
        }

        public static StateMachineSubscriptionConfigurator<TInstance> StateMachineSaga<TInstance>(
            this IReceiveEndpointConfigurator configurator, StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> sagaRepository,
            Action<StateMachineSagaRepositoryConfigurator<TInstance>> configureCallback)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineSagaRepositoryConfigurator =
                new StateMachineSagaRepositoryConfiguratorImpl<TInstance>(stateMachine, sagaRepository);

            configureCallback(stateMachineSagaRepositoryConfigurator);

            StateMachineSagaRepository<TInstance> repository = stateMachineSagaRepositoryConfigurator.Configure();

            var stateMachineConfigurator = new StateMachineSubscriptionConfiguratorImpl<TInstance>(stateMachine, repository);

            configurator.AddConfigurator(stateMachineConfigurator);

            return stateMachineConfigurator;
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IBus bus, StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> sagaRepository)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineSagaRepositoryConfigurator = new StateMachineSagaRepositoryConfiguratorImpl<TInstance>(stateMachine,
                sagaRepository);

            StateMachineSagaRepository<TInstance> repository = stateMachineSagaRepositoryConfigurator.Configure();

            var connector = new StateMachineConnector<TInstance>(stateMachine, repository);

            return connector.Connect(bus.ConsumePipe, repository, Retry.None);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IBus bus, StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> sagaRepository, Action<StateMachineSagaRepositoryConfigurator<TInstance>> configureCallback)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineSagaRepositoryConfigurator = new StateMachineSagaRepositoryConfiguratorImpl<TInstance>(stateMachine,
                sagaRepository);

            configureCallback(stateMachineSagaRepositoryConfigurator);

            StateMachineSagaRepository<TInstance> repository = stateMachineSagaRepositoryConfigurator.Configure();

            var connector = new StateMachineConnector<TInstance>(stateMachine, repository);

            return connector.Connect(bus.ConsumePipe, repository, Retry.None);
        }
    }
}