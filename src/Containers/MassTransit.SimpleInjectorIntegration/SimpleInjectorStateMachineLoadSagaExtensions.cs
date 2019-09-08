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
    using System.Collections.Generic;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Registration;
    using AutomatonymousSimpleInjectorIntegration;
    using AutomatonymousSimpleInjectorIntegration.Registration;
    using GreenPipes.Internals.Extensions;
    using SimpleInjector;
    using Registration;
    using SimpleInjectorIntegration;
    using SimpleInjectorIntegration.Registration;


    public static class SimpleInjectorStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Specify that the service bus should load the StateMachineSagas from the container passed as argument
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, Container container)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (container == null)
                throw new ArgumentNullException(nameof(container));


            var registrationConfigurator = new RegistrationConfigurator();

            container.RegisterInstance<ISagaStateMachineFactory>(new SimpleInjectorSagaStateMachineFactory(container));
            container.RegisterInstance<IStateMachineActivityFactory>(new SimpleInjectorStateMachineActivityFactory());
            container.RegisterInstance<ISagaRepositoryFactory>(new SimpleInjectorSagaRepositoryFactory(container));

            IEnumerable<Type> sagaTypes = FindStateMachineSagaTypes(container);
            foreach (var sagaType in sagaTypes)
                SagaStateMachineRegistrationCache.AddSagaStateMachine(registrationConfigurator, sagaType);

            var registration = registrationConfigurator.CreateRegistration(new SimpleInjectorConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        static IEnumerable<Type> FindStateMachineSagaTypes(Container container)
        {
            return
                container.GetCurrentRegistrations()
                    .Where(r => r.Registration.ImplementationType.HasInterface(typeof(SagaStateMachine<>)))
                    .Select(x => x.Registration.ImplementationType)
                    .ToList();
        }
    }
}
