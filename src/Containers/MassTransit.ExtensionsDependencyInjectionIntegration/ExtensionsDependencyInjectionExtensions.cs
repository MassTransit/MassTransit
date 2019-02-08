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
    using ExtensionsDependencyInjectionIntegration.ScopeProviders;
    using GreenPipes;
    using GreenPipes.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Pipeline.Filters;
    using Saga;


    public static class ExtensionsDependencyInjectionIntegrationExtensions
    {
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider)
        {
            var registryConfigurator = serviceProvider.GetRequiredService<IRegistration>();

            registryConfigurator.ConfigureConsumers(configurator);
            registryConfigurator.ConfigureSagas(configurator);
        }

        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="collection"></param>
        public static void RegisterInMemorySagaRepository(this ServiceCollection collection)
        {
            collection.AddSingleton(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>));
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this ServiceCollection collection)
            where T : class, ISaga
        {
            collection.AddSingleton<ISagaRepository<T>, InMemorySagaRepository<T>>();
        }

        /// <summary>
        /// Creates a scope which is used by all downstream filters/consumers/etc.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="serviceProvider"></param>
        public static void UseServiceScope(this IPipeConfigurator<ConsumeContext> configurator, IServiceProvider serviceProvider)
        {
            var scopeProvider = new DependencyInjectionConsumerScopeProvider(serviceProvider);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeFilter(scopeProvider));

            configurator.AddPipeSpecification(specification);
        }
    }
}
