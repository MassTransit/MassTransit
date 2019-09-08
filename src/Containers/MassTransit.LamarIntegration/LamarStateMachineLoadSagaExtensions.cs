// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Registration;
    using GreenPipes.Internals.Extensions;
    using Lamar;
    using LamarIntegration;
    using LamarIntegration.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;


    public static class LamarStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Scans the lifetime scope and registers any state machines sagas which are found in the scope using the StructureMap saga repository
        /// and the appropriate state machine saga repository under the hood.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IContainer container)
        {
            var registrationConfigurator = new RegistrationConfigurator();

            container.Configure(x =>
            {
                if (container.TryGetInstance<ISagaStateMachineFactory>() == null)
                    x.AddSingleton<ISagaStateMachineFactory>(provider => new LamarSagaStateMachineFactory(provider));

                if (container.TryGetInstance<ISagaRepositoryFactory>() == null)
                    x.AddSingleton<ISagaRepositoryFactory>(provider => new LamarSagaRepositoryFactory(container));

                if (container.TryGetInstance<IStateMachineActivityFactory>() == null)
                    x.AddSingleton<IStateMachineActivityFactory>(provider => new LamarStateMachineActivityFactory());
            });

            registrationConfigurator.AddSagaStateMachines(new NullSagaStateMachineRegistrar(), FindStateMachineSagaTypes(container));

            var registration = registrationConfigurator.CreateRegistration(new LamarConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        public static Type[] FindStateMachineSagaTypes(IContainer container)
        {
            return container
                .Model
                .AllInstances.ToArray()
                .Where(x => x.ImplementationType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(x => x.ImplementationType)
                .Distinct()
                .ToArray();
        }
    }
}
