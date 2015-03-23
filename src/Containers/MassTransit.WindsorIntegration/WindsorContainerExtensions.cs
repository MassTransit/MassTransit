// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Castle.Windsor;
    using ConsumeConfigurators;
    using Internals.Extensions;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using WindsorIntegration;


    /// <summary>
    /// Extension methods for the Windsor IoC container.
    /// </summary>
    public static class WindsorContainerExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The Windsor container.</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IWindsorContainer container)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");
            if (container == null)
                throw new ArgumentNullException("container");

            IList<Type> consumerTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (consumerTypes.Count > 0)
            {
                foreach (Type type in consumerTypes)
                    ConsumerConfiguratorCache.Configure(type, configurator, container);
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
            {
                foreach (Type sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, container);
            }
        }

        /// <summary>
        /// Register the type as a type to load from the container as a consumer.
        /// </summary>
        /// <typeparam name="TConsumer">The type of the consumer that consumes messages</typeparam>
        /// <param name="configurator">configurator</param>
        /// <param name="container">The container that the consumer should be loaded from.</param>
        /// <returns>The configurator</returns>
        public static IConsumerConfigurator<TConsumer> Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator,
            IWindsorContainer container)
            where TConsumer : class, IConsumer
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");
            if (container == null)
                throw new ArgumentNullException("container");

            var consumerFactory = new WindsorConsumerFactory<TConsumer>(container);

            return configurator.Consumer(consumerFactory);
        }

        /// <summary>
        /// Load the saga of the generic type from the windsor container,
        /// by loading it directly from the container.
        /// </summary>
        /// <typeparam name="TSaga">The type of the saga</typeparam>
        /// <param name="configurator">The configurator</param>
        /// <param name="container">The windsor container</param>
        /// <returns>The configurator</returns>
        public static ISagaConfigurator<TSaga> Saga<TSaga>(this IReceiveEndpointConfigurator configurator,
            IWindsorContainer container)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");
            if (container == null)
                throw new ArgumentNullException("container");

            var sagaRepository = container.Resolve<ISagaRepository<TSaga>>();

            var windsorSagaRepository = new WindsorSagaRepository<TSaga>(sagaRepository, container);

            return configurator.Saga(windsorSagaRepository);
        }

        /// <summary>
        /// Enables message scope lifetime for windsor containers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        public static void EnableMessageScope<T>(this IPipeConfigurator<T> configurator)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            var pipeBuilderConfigurator = new WindsorMessageScopePipeSpecification<T>();

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        static IList<Type> FindTypes<T>(IWindsorContainer container, Func<Type, bool> filter)
        {
            return container.Kernel
                .GetAssignableHandlers(typeof(T))
                .Select(h => h.ComponentModel.Implementation)
                .Where(filter)
                .ToList();
        }
    }
}