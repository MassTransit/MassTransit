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
    using System.Reflection;
    using Automatonymous;
    using Automatonymous.Registration;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public static class DependencyInjectionStateMachineSagaRegistrationExtensions
    {
        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="type">The state machine saga type</param>
        public static void RegisterSagaStateMachine(this IServiceCollection collection, Type type)
        {
            var registrar = new DependencyInjectionSagaStateMachineRegistrar(collection);

            SagaStateMachineRegistrationCache.Register(type, registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="collection"></param>
        public static void RegisterSagaStateMachine<TStateMachine, TInstance>(this IServiceCollection collection)
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var registrar = new DependencyInjectionSagaStateMachineRegistrar(collection);

            SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="assemblies">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this IServiceCollection collection, params Assembly[] assemblies)
        {
            var registrar = new DependencyInjectionSagaStateMachineRegistrar(collection);

            registrar.RegisterSagaStateMachines(assemblies);
        }

        /// <summary>
        /// Add the state machine sagas by type
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="types">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this IServiceCollection collection, params Type[] types)
        {
            var registrar = new DependencyInjectionSagaStateMachineRegistrar(collection);

            registrar.RegisterSagaStateMachines(types);
        }
    }
}
