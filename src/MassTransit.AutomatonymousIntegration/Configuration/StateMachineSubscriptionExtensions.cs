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
namespace Automatonymous
{
    using System;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.SubscriptionConfigurators;
    using RepositoryConfigurators;
    using SubscriptionConfigurators;
    using SubscriptionConnectors;


    public static class StateMachineSubscriptionExtensions
    {
        public static StateMachineSubscriptionConfigurator<TInstance> StateMachineSaga<TInstance>(
            this IReceiveEndpointConfigurator configurator, StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> sagaRepository)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineSagaRepositoryConfigurator =
                new StateMachineSagaRepositoryConfiguratorImpl<TInstance>(stateMachine, sagaRepository);

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

            var stateMachineConfigurator = new StateMachineSubscriptionConfiguratorImpl<TInstance>(stateMachine,
                repository);

            configurator.AddConfigurator(stateMachineConfigurator);

            return stateMachineConfigurator;
        }

        public static UnsubscribeAction SubscribeStateMachineSaga<TInstance>(
            this IServiceBus bus, StateMachine<TInstance> stateMachine, ISagaRepository<TInstance> sagaRepository)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineSagaRepositoryConfigurator =
                new StateMachineSagaRepositoryConfiguratorImpl<TInstance>(stateMachine, sagaRepository);

            StateMachineSagaRepository<TInstance> repository = stateMachineSagaRepositoryConfigurator.Configure();

            var connector = new StateMachineConnector<TInstance>(stateMachine, repository);

            return bus.Configure(x => connector.Connect(x));
        }

        public static UnsubscribeAction SubscribeStateMachineSaga<TInstance>(
            this IServiceBus bus, StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> sagaRepository,
            Action<StateMachineSagaRepositoryConfigurator<TInstance>> configureCallback)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineSagaRepositoryConfigurator =
                new StateMachineSagaRepositoryConfiguratorImpl<TInstance>(stateMachine, sagaRepository);

            configureCallback(stateMachineSagaRepositoryConfigurator);

            StateMachineSagaRepository<TInstance> repository = stateMachineSagaRepositoryConfigurator.Configure();

            var connector = new StateMachineConnector<TInstance>(stateMachine, repository);

            return bus.Configure(x => connector.Connect(x));
        }
    }
}