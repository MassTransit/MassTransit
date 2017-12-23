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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.MicroKernel;
    using Castle.Windsor;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;
    using Internals.Extensions;
    using PipeConfigurators;
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
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            LoadFrom(configurator, container.Kernel);
        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The Windsor container.</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IKernel container)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            IList<Type> consumerTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (consumerTypes.Count > 0)
            {
                foreach (var type in consumerTypes)
                    ConsumerConfiguratorCache.Configure(type, configurator, container);
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
            {
                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, container);
            }
        }

        /// <summary>
        /// Register the type as a type to load from the container as a consumer.
        /// </summary>
        /// <typeparam name="T">The type of the consumer that consumes messages</typeparam>
        /// <param name="configurator">configurator</param>
        /// <param name="container">The container that the consumer should be loaded from.</param>
        /// <param name="configure"></param>
        /// <returns>The configurator</returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IKernel container, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var consumerFactory = new WindsorConsumerFactory<T>(container);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Load the saga of the generic type from the windsor container,
        /// by loading it directly from the container.
        /// </summary>
        /// <typeparam name="T">The type of the saga</typeparam>
        /// <param name="configurator">The configurator</param>
        /// <param name="container">The windsor container</param>
        /// <param name="configure"></param>
        /// <returns>The configurator</returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IKernel container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var sagaRepository = container.Resolve<ISagaRepository<T>>();

            var windsorSagaRepository = new WindsorSagaRepository<T>(sagaRepository, container);

            configurator.Saga(windsorSagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator,
            Uri compensateAddress, IKernel kernel)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var factory = new WindsorExecuteActivityFactory<TActivity, TArguments>(kernel);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator, IKernel kernel)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var factory = new WindsorExecuteActivityFactory<TActivity, TArguments>(kernel);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IKernel kernel)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var factory = new WindsorCompensateActivityFactory<TActivity, TLog>(kernel);
            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configurator.AddEndpointSpecification(specification);
        }

        /// <summary>
        /// Enables message scope lifetime for windsor containers
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Change to UseMessageScope - method was renamed to match middleware conventions")]
        public static void EnableMessageScope(this IConsumePipeConfigurator configurator)
        {
            UseMessageScope(configurator);
        }

        /// <summary>
        /// Enables message scope lifetime for windsor containers
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseMessageScope(this IConsumePipeConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new WindsorMessageScopePipeSpecification();

            configurator.AddPrePipeSpecification(specification);
        }

        static IList<Type> FindTypes<T>(IKernel container, Func<Type, bool> filter)
        {
            return container
                .GetAssignableHandlers(typeof(T))
                .Select(h => h.ComponentModel.Services.First())
                .Where(filter)
                .ToList();
        }
    }
}