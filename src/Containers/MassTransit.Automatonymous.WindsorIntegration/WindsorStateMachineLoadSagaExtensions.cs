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
    using AutomatonymousWindsorIntegration;
    using AutomatonymousWindsorIntegration.Registration;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Registration;
    using WindsorIntegration.Registration;


    public static class WindsorStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Specify that the service bus should load the StateMachineSagas from the container passed as argument
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IWindsorContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            LoadStateMachineSagas(configurator, container.Kernel);
        }

        /// <summary>
        /// Specify that the service bus should load the StateMachineSagas from the container passed as argument
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IKernel container)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (container == null)
                throw new ArgumentNullException(nameof(container));


            var registrationConfigurator = new RegistrationConfigurator();

            container.Register(Component.For<ISagaStateMachineFactory>().Instance(new WindsorSagaStateMachineFactory(container)));
            container.Register(Component.For<IStateMachineActivityFactory>().Instance(new WindsorStateMachineActivityFactory()));
            container.Register(Component.For<ISagaRepositoryFactory>().Instance(new WindsorSagaRepositoryFactory(container)));

            IEnumerable<Type> sagaTypes = FindStateMachineSagaTypes(container);
            foreach (var sagaType in sagaTypes)
                SagaStateMachineRegistrationCache.AddSagaStateMachine(registrationConfigurator, sagaType);

            var registration = registrationConfigurator.CreateRegistration(new WindsorConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        static IEnumerable<Type> FindStateMachineSagaTypes(IKernel container)
        {
            List<Type> types = container.GetAssignableHandlers(typeof(StateMachine))
                .Where(x => x.HasInterface(typeof(SagaStateMachine<>)))
                .Select(x => x.ComponentModel.Implementation)
                .Distinct()
                .ToList();

            return types;
        }

        static bool HasInterface(this IHandler handler, Type type)
        {
            return handler.ComponentModel.Implementation.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type);
        }
    }
}
