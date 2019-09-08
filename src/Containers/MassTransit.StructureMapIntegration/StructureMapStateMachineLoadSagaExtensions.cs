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
namespace MassTransit
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Registration;
    using Internals.Extensions;
    using Registration;
    using StructureMap;
    using StructureMapIntegration;
    using StructureMapIntegration.Registration;


    public static class StructureMapStateMachineLoadSagaExtensions
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
                    x.For<ISagaStateMachineFactory>().Use(provider => new StructureMapSagaStateMachineFactory(container)).Singleton();

                if (container.TryGetInstance<ISagaRepositoryFactory>() == null)
                    x.For<ISagaRepositoryFactory>().Use(provider => new StructureMapSagaRepositoryFactory(container)).Singleton();

                if (container.TryGetInstance<IStateMachineActivityFactory>() == null)
                    x.For<IStateMachineActivityFactory>().Use(provider => new StructureMapStateMachineActivityFactory()).ContainerScoped();
            });

            registrationConfigurator.AddSagaStateMachines(new NullSagaStateMachineRegistrar(), FindStateMachineSagaTypes(container));

            var registration = registrationConfigurator.CreateRegistration(new StructureMapConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        public static Type[] FindStateMachineSagaTypes(IContainer container)
        {
            return container
                .Model
                .PluginTypes
                .Where(x => x.PluginType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(i => i.PluginType)
                .Distinct()
                .ToArray();
        }
    }
}
