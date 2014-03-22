// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using BusConfigurators;
    using Castle.Windsor;
    using Magnum.Extensions;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using SubscriptionConfigurators;
    using Util;
    using WindsorIntegration;
    using Castle.MicroKernel;


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
        public static void LoadFrom(
            [NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] IWindsorContainer container)
        {
            configurator.LoadFrom(container, false);
        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument,
        /// using only those subscribers which are explicitly registered as an IConsumer or ISaga service.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The Windsor container.</param>
        public static void LoadRegisteredServicesFrom(
            [NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] IWindsorContainer container)
        {
            configurator.LoadFrom(container, true);
        }

        static void LoadFrom(
            [NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] IWindsorContainer container,
            bool useOnlyRegisteredServices)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");
            if (container == null)
                throw new ArgumentNullException("container");

            IList<Type> consumerTypes = container.FindTypes<IConsumer>(x => !x.Implements<ISaga>(), useOnlyRegisteredServices);
            if (consumerTypes.Count > 0)
            {
                var consumerConfigurator = new WindsorConsumerFactoryConfigurator(configurator, container);

                foreach (Type type in consumerTypes)
                    consumerConfigurator.ConfigureConsumer(type);
            }

            IList<Type> sagaTypes = container.FindTypes<ISaga>(x => true, useOnlyRegisteredServices);
            if (sagaTypes.Count > 0)
            {
                var sagaConfigurator = new WindsorSagaFactoryConfigurator(configurator, container);

                foreach (Type type in sagaTypes)
                    sagaConfigurator.ConfigureSaga(type);
            }
        }

        /// <summary>
        /// Register the type as a type to load from the container as a consumer.
        /// </summary>
        /// <typeparam name="TConsumer">The type of the consumer that consumes messages</typeparam>
        /// <param name="configurator">configurator</param>
        /// <param name="container">The container that the consumer should be loaded from.</param>
        /// <returns>The configurator</returns>
        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
            [NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] IWindsorContainer container)
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
        public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
            [NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] IWindsorContainer container)
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
        /// Enable the begin/end of a MessageScope for use with the container
        /// </summary>
        /// <param name="configurator"></param>
        public static void EnableMessageScope(this ServiceBusConfigurator configurator)
        {
            configurator.AddInboundInterceptor(new WindsorInboundInterceptor());
        }

        /// <summary>
        /// Pulls Types from the container.
        /// </summary>
        /// <param name="useOnlyRegisteredServices">If true, pulls only Types registered as services. If false, pulls all Types registered which are assignable from the given Type.</param>
        static IList<Type> FindTypes<T>(
                this IWindsorContainer container, 
                Func<Type, bool> filter, 
                bool useOnlyRegisteredServices)
        {
            IHandler[] handlers;
            if (!useOnlyRegisteredServices)
            {
                handlers = container.Kernel.GetAssignableHandlers(typeof(T));
            }
            else
            {
                handlers = container.Kernel.GetHandlers(typeof(T));
            }

            return handlers
                    .Select(h => h.ComponentModel.Implementation)
                    .Where(filter)
                    .ToList();
        }
    }
}