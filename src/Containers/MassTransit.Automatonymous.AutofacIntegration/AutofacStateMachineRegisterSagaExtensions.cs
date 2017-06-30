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
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using Autofac.Builder;
    using Autofac.Features.Scanning;
    using Automatonymous;


    public static class AutofacStateMachineRegisterSagaExtensions
    {
        /// <summary>
        /// Register the state machine sagas found in the specified assemblies using the ContainerBuilder provided. The
        /// machines are registered using their SagaStateMachine&lt;&gt; type, as well as the concrete type for use by
        /// the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> RegisterStateMachineSagas(this ContainerBuilder builder,
            params Assembly[] assemblies)
        {
            return builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(SagaStateMachine<>)))
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}