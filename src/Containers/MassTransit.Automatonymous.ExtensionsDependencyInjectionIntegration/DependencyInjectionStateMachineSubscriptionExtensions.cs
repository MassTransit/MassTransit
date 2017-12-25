// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using Automatonymous;
    using Automatonymous.Scoping;
    using Automatonymous.SubscriptionConfigurators;
    using Automatonymous.SubscriptionConnectors;
    using AutomatonymousExtensionsDependencyInjectionIntegration;
    using GreenPipes;
    using Pipeline;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using Scoping;


    public static class DependencyInjectionStateMachineSubscriptionExtensions
    {
        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="scope">The StructureMap Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IServiceProvider scope, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepositoryFactory repositoryFactory = new DependencyInjectionStateMachineSagaRepositoryFactory(scope);

            var sagaRepository = repositoryFactory.CreateSagaRepository<TInstance>();

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, sagaRepository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope">The StructureMap Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IServiceProvider scope, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new DependencyInjectionSagaStateMachineFactory(scope);

            var stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            ISagaRepositoryFactory repositoryFactory = new DependencyInjectionStateMachineSagaRepositoryFactory(scope);

            var sagaRepository = repositoryFactory.CreateSagaRepository<TInstance>();

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, sagaRepository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine, IServiceProvider scope)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaRepositoryFactory repositoryFactory = new DependencyInjectionStateMachineSagaRepositoryFactory(scope);

            var sagaRepository = repositoryFactory.CreateSagaRepository<TInstance>();

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            return connector.ConnectSaga(bus, sagaRepository, specification);
        }
    }
}