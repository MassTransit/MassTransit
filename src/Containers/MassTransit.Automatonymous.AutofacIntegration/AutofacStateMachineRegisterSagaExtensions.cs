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
    using Autofac;
    using Automatonymous;
    using Automatonymous.Registration;
    using AutomatonymousAutofacIntegration.Registration;


    public static class AutofacStateMachineRegisterSagaExtensions
    {
        /// <summary>
        /// Register the state machine sagas found in the specified assemblies using the ContainerBuilder provided. The
        /// machines are registered using their SagaStateMachine&lt;&gt; type, as well as the concrete type for use by
        /// the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        [Obsolete("use RegisterSagaStateMachines instead, this method now forwards to that one")]
        public static void RegisterStateMachineSagas(this ContainerBuilder builder,
            params Assembly[] assemblies)
        {
            RegisterSagaStateMachines(builder, assemblies);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="type">The state machine saga type</param>
        public static void RegisterSagaStateMachine(this ContainerBuilder builder, Type type)
        {
            var registrar = new AutofacSagaStateMachineRegistrar(builder);

            SagaStateMachineRegistrationCache.Register(type, registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterSagaStateMachine<TStateMachine, TInstance>(this ContainerBuilder builder)
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var registrar = new AutofacSagaStateMachineRegistrar(builder);

            SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            var registrar = new AutofacSagaStateMachineRegistrar(builder);

            registrar.RegisterSagaStateMachines(assemblies);
        }

        /// <summary>
        /// Add the state machine sagas by type
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="types">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this ContainerBuilder builder, params Type[] types)
        {
            var registrar = new AutofacSagaStateMachineRegistrar(builder);

            registrar.RegisterSagaStateMachines(types);
        }
    }
}
