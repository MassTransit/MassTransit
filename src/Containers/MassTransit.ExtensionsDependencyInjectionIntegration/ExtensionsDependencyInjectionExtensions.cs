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
    using ConsumeConfigurators;
    using Courier;
    using ExtensionsDependencyInjectionIntegration;
    using GreenPipes;
    using GreenPipes.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using PipeConfigurators;
    using Pipeline.Filters;
    using Saga;
    using Scoping;


    public static class ExtensionsDependencyInjectionIntegrationExtensions
    {
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider)
        {
            var consumerCache = serviceProvider.GetService<IConsumerCacheService>();

            var consumers = consumerCache.GetConfigurators();

            foreach (var consumer in consumers)
                consumer.Configure(configurator, serviceProvider);

            var sagaCache = serviceProvider.GetService<ISagaCacheService>();

            IEnumerable<ICachedConfigurator> sagas = sagaCache.GetConfigurators();

            foreach (var saga in sagas)
                saga.Configure(configurator, serviceProvider);
        }

        /// <summary>
        /// Registers a consumer which is resolved from the service provider, within a container scope
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="serviceProvider">The LifetimeScope of the container</param>
        /// <param name="configure">Configure the consumer (optional)</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new DependencyInjectionConsumerScopeProvider(serviceProvider);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var sagaRepository = serviceProvider.GetRequiredService<ISagaRepository<T>>();

            var repository = new ScopeSagaRepository<T>(sagaRepository, new DependencyInjectionSagaScopeProvider<T>(serviceProvider));

            configurator.Saga(repository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress, IServiceProvider serviceProvider,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>(serviceProvider);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>(serviceProvider);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new DependencyInjectionCompensateActivityScopeProvider<TActivity, TLog>(serviceProvider);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
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