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
    using System.Linq.Expressions;
    using Autofac;
    using Autofac.Builder;
    using Autofac.Core;
    using AutofacIntegration;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;
    using GreenPipes.Specifications;
    using Internals.Extensions;
    using PipeConfigurators;
    using Pipeline.Filters;
    using Saga;
    using Scoping;


    public static class AutofacExtensions
    {
        /// <summary>
        /// Load the consumer configuration from the specified Autofac LifetimeScope
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="scope">The LifetimeScope of the container</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(scope);

            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(lifetimeScopeProvider, name, scopeConfigurator);

            IList<Type> concreteTypes = FindTypes(scope, r => !r.HasInterface<ISaga>(), typeof(IConsumer));
            if (concreteTypes.Count > 0)
                foreach (var concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, scopeProvider);

            var sagaRepositoryFactory = new AutofacSagaRepositoryFactory(lifetimeScopeProvider, name, scopeConfigurator);

            IList<Type> sagaTypes = FindTypes(scope, x => true, typeof(ISaga));
            if (sagaTypes.Count > 0)
                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, sagaRepositoryFactory);
        }

        /// <summary>
        /// Load the consumer configuration from the specified Autofac LifetimeScope
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">The component context of the container</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
        {
            var scope = context.Resolve<ILifetimeScope>();

            LoadFrom(configurator, scope, name, scopeConfigurator);
        }

        /// <summary>
        /// Creates a lifetime scope which is shared by any downstream filters (rather than creating a new one).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="name">The name of the lifetime scope</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        public static void UseLifetimeScope(this IPipeConfigurator<ConsumeContext> configurator, ILifetimeScope lifetimeScope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
        {
            var scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(lifetimeScope), name, scopeConfigurator);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeFilter(scopeProvider));

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="scope">The LifetimeScope of the container</param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(scope), name, scopeConfigurator);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The component context containing the registry</param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void ConsumerInScope<T, TId>(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, IConsumer
        {
            ILifetimeScopeRegistry<TId> registry = context.Resolve<ILifetimeScopeRegistry<TId>>();

            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new RegistryLifetimeScopeProvider<TId>(registry), name, scopeConfigurator);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The LifetimeScope of the container</param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, IConsumer
        {
            var lifetimeScope = context.Resolve<ILifetimeScope>();

            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(lifetimeScope), name, scopeConfigurator);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="scope">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, Action<IConsumerConfigurator<T>> configure, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(scope), name, scopeConfigurator);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, Action<IConsumerConfigurator<T>> configure,
            string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, IConsumer
        {
            var scope = context.Resolve<ILifetimeScope>();

            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(scope), name, scopeConfigurator);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope"></param>
        /// <param name="name"></param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, ISaga
        {
            var repository = scope.Resolve<ISagaRepository<T>>();

            ISagaScopeProvider<T> scopeProvider = new AutofacSagaScopeProvider<T>(new SingleLifetimeScopeProvider(scope), name, scopeConfigurator);

            var sagaRepository = new ScopeSagaRepository<T>(repository, scopeProvider);

            configurator.Saga(sagaRepository);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, ISaga
        {
            var scope = context.Resolve<ILifetimeScope>();

            var repository = scope.Resolve<ISagaRepository<T>>();

            ISagaScopeProvider<T> scopeProvider = new AutofacSagaScopeProvider<T>(new SingleLifetimeScopeProvider(scope), name, scopeConfigurator);

            var sagaRepository = new ScopeSagaRepository<T>(repository, scopeProvider);

            configurator.Saga(sagaRepository);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope"></param>
        /// <param name="configure"></param>
        /// <param name="name"></param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, Action<ISagaConfigurator<T>> configure, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, ISaga
        {
            var repository = scope.Resolve<ISagaRepository<T>>();

            ISagaScopeProvider<T> scopeProvider = new AutofacSagaScopeProvider<T>(new SingleLifetimeScopeProvider(scope), name, scopeConfigurator);

            var sagaRepository = new ScopeSagaRepository<T>(repository, scopeProvider);

            configurator.Saga(sagaRepository, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        /// <param name="name"></param>
        /// <param name="scopeConfigurator">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, Action<ISagaConfigurator<T>> configure, string name = "message",
            Action<ContainerBuilder, ConsumeContext> scopeConfigurator = null)
            where T : class, ISaga
        {
            var scope = context.Resolve<ILifetimeScope>();

            var repository = scope.Resolve<ISagaRepository<T>>();

            ISagaScopeProvider<T> scopeProvider = new AutofacSagaScopeProvider<T>(new SingleLifetimeScopeProvider(scope), name, scopeConfigurator);

            var sagaRepository = new ScopeSagaRepository<T>(repository, scopeProvider);

            configurator.Saga(sagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress, ILifetimeScope lifetimeScope,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> scopeConfigurator = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var executeActivityScopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, name, scopeConfigurator);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope, string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> scopeConfigurator = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var executeActivityScopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, name, scopeConfigurator);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope, string name = "message",
            Action<ContainerBuilder, CompensateContext<TLog>> scopeConfigurator = null)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var compensateActivityScopeProvider = new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, name, scopeConfigurator);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configurator.AddEndpointSpecification(specification);
        }

        /// <summary>
        /// Register an accessor for an input type in the container
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="propertyExpression"></param>
        public static IRegistrationBuilder<ILifetimeScopeIdAccessor<TInput, T>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterLifetimeScopeIdAccessor<TInput, T>(this ContainerBuilder builder, Expression<Func<TInput, T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            var propertyInfo = propertyExpression.GetPropertyInfo();

            return builder.RegisterType<MessageLifetimeScopeIdAccessor<TInput, T>>()
                .As<ILifetimeScopeIdAccessor<TInput, T>>()
                .WithParameter(TypedParameter.From(propertyInfo));
        }

        /// <summary>
        /// Register a lifetime scope registry for the given identifier type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="scopeTag"></param>
        /// <returns></returns>
        public static IRegistrationBuilder<ILifetimeScopeRegistry<string>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterLifetimeScopeRegistry<T>(this ContainerBuilder builder, object scopeTag)
        {
            return builder.RegisterType<LifetimeScopeRegistry<string>>()
                .As<ILifetimeScopeRegistry<string>>()
                .WithParameter("tag", scopeTag)
                .SingleInstance();
        }

        public static IList<Type> FindTypes(IComponentContext scope, Func<Type, bool> filter, Type interfaceType)
        {
            return scope.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new { r, s })
                .Where(rs => rs.s.ServiceType.HasInterface(interfaceType))
                .Select(rs => rs.s.ServiceType)
                .Where(filter)
                .ToList();
        }
    }
}