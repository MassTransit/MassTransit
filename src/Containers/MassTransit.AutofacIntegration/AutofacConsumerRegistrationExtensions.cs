// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Reflection;
    using Autofac.Builder;
    using Autofac.Features.Scanning;
    using Autofac;
    using Saga;


    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with methods to support MassTransit.
    /// </summary>
    public static class AutofacConsumerRegistrationExtensions
    {
        /// <summary>
        /// Register types that implement <see cref="IConsumer"/> in the provided assemblies.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="consumerAssemblies">Assemblies to scan for consumers.</param>
        /// <returns>Registration builder allowing the consumer components to be customised.</returns>
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            RegisterConsumers(this ContainerBuilder builder, params Assembly[] consumerAssemblies)
        {
            return builder.RegisterAssemblyTypes(consumerAssemblies)
                .Where(t => typeof(IConsumer).IsAssignableFrom(t));
        }


        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterInMemorySagaRepository(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(InMemorySagaRepository<>))
                .As(typeof(ISagaRepository<>))
                .SingleInstance();
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this ContainerBuilder builder)
            where T : class, ISaga
        {
            builder.RegisterType<InMemorySagaRepository<T>>()
                .As<ISagaRepository<T>>()
                .SingleInstance();
        }
    }
}
