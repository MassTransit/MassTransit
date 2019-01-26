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
    using System.Reflection;
    using Automatonymous;
    using Automatonymous.Registration;
    using Internals.Extensions;
    using Registration;
    using Util;


    public static class AutomatonymousRegistrationExtensions
    {
        /// <summary>
        /// Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registrar"></param>
        /// <typeparam name="TStateMachine"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        public static void AddSagaStateMachine<TStateMachine, TInstance>(this IRegistrationConfigurator configurator, ISagaStateMachineRegistrar registrar)
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRegistration Factory()
            {
                SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), registrar);

                return new SagaStateMachineRegistration<TInstance>();
            }

            configurator.AddSaga<TInstance>(Factory);
        }

        /// <summary>
        /// Adds SagaStateMachines to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registrar"></param>
        /// <param name="assemblies">The assemblies to scan for state machines</param>
        public static void AddSagaStateMachines(this IRegistrationConfigurator configurator, ISagaStateMachineRegistrar registrar, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            TypeSet types = AssemblyTypeCache.FindTypes(assemblies, x => x.HasInterface(typeof(SagaStateMachine<>))).GetAwaiter().GetResult();

            configurator.AddSagaStateMachines(registrar, types.FindTypes(TypeClassification.Concrete).ToArray());
        }

        /// <summary>
        /// Adds SagaStateMachines to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registrar"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddSagaStateMachines(this IRegistrationConfigurator configurator, ISagaStateMachineRegistrar registrar, params Type[] types)
        {
            foreach (var type in types)
            {
                if (!type.HasInterface(typeof(SagaStateMachine<>)))
                    throw new ArgumentException($"The type is not a saga state machine: {TypeMetadataCache.GetShortName(type)}", nameof(types));

                configurator.AddSagaStateMachine(type, registrar);
            }
        }

        public static void RegisterSagaStateMachines(this ISagaStateMachineRegistrar registrar, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            TypeSet types = AssemblyTypeCache.FindTypes(assemblies, x => x.HasInterface(typeof(SagaStateMachine<>))).GetAwaiter().GetResult();

            registrar.RegisterSagaStateMachines(types.FindTypes(TypeClassification.Concrete).ToArray());
        }

        public static void RegisterSagaStateMachines(this ISagaStateMachineRegistrar registrar, params Type[] types)
        {
            foreach (var type in types)
            {
                if (!type.HasInterface(typeof(SagaStateMachine<>)))
                    throw new ArgumentException($"The type is not a saga state machine: {TypeMetadataCache.GetShortName(type)}", nameof(types));

                SagaStateMachineRegistrationCache.Register(type, registrar);
            }
        }
    }
}
