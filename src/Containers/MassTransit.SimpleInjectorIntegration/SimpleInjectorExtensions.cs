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
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using Courier;
    using Internals.Extensions;
    using PipeConfigurators;
    using Registration;
    using Saga;
    using Scoping;
    using SimpleInjector;
    using SimpleInjectorIntegration;
    using SimpleInjectorIntegration.ScopeProviders;


    public static class SimpleInjectorExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The SimpleInjector container.</param>
        /// <remarks>You should register your message consumers with AsyncScoped lifestyle.</remarks>
        [Obsolete("This method is not recommended, since it may load multiple consumers into a single receive endpoint. Review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, Container container)
        {
            var consumerScopeProvider = new SimpleInjectorConsumerScopeProvider(container);

            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (concreteTypes.Count > 0)
                foreach (var concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, consumerScopeProvider);

            var repositoryFactory = new SimpleInjectorSagaRepositoryFactory(container);

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, repositoryFactory);
        }

        static IList<Type> FindTypes<T>(Container container, Func<Type, bool> filter)
        {
            return
                container.GetCurrentRegistrations()
                    .Where(r => r.Registration.ImplementationType.HasInterface<T>())
                    .Select(x => x.Registration.ImplementationType)
                    .Where(filter)
                    .ToList();
        }

        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="registry"></param>
        public static void RegisterInMemorySagaRepository(this Container registry)
        {
            registry.RegisterSingleton(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>));
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this Container registry)
            where T : class, ISaga
        {
            registry.RegisterSingleton<ISagaRepository<T>, InMemorySagaRepository<T>>();
        }
    }
}
