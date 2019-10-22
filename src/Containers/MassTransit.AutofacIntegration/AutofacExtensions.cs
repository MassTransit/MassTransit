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
    using AutofacIntegration.Registration;
    using AutofacIntegration.ScopeProviders;
    using GreenPipes;
    using GreenPipes.Specifications;
    using Internals.Extensions;
    using Pipeline.Filters;
    using Registration;
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
        /// <param name="configureScope">Configuration for scope container</param>
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
        {
            var registration = scope.ResolveOptional<IRegistration>();
            if (registration != null)
            {
                registration.ConfigureConsumers(configurator);
                registration.ConfigureSagas(configurator);

                return;
            }

            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(scope);

            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(lifetimeScopeProvider, name, configureScope);

            IList<Type> concreteTypes = FindTypes(scope, r => !r.HasInterface<ISaga>(), typeof(IConsumer));
            if (concreteTypes.Count > 0)
            {
                foreach (var concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, scopeProvider);
            }

            var sagaRepositoryFactory = new AutofacSagaRepositoryFactory(lifetimeScopeProvider, name, configureScope);

            IList<Type> sagaTypes = FindTypes(scope, x => true, typeof(ISaga));
            if (sagaTypes.Count > 0)
            {
                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, sagaRepositoryFactory);
            }
        }

        /// <summary>
        /// Load the consumer configuration from the specified Autofac LifetimeScope
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">The component context of the container</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        [Obsolete(
            "This method is not recommended, since it may load multiple consumers into a single receive endpoint. Review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
        {
            var registration = context.ResolveOptional<IRegistration>();
            if (registration != null)
            {
                registration.ConfigureConsumers(configurator);
                registration.ConfigureSagas(configurator);

                return;
            }

            LoadFrom(configurator, context.Resolve<ILifetimeScope>(), name, configureScope);
        }

        /// <summary>
        /// Creates a lifetime scope which is shared by any downstream filters (rather than creating a new one).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="name">The name of the lifetime scope</param>
        /// <param name="configureScope">Configuration for scope container</param>
        public static void UseLifetimeScope(this IPipeConfigurator<ConsumeContext> configurator, ILifetimeScope lifetimeScope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
        {
            var scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(lifetimeScope), name, configureScope);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeFilter(scopeProvider));

            configurator.AddPipeSpecification(specification);
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
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new
                {
                    r,
                    s
                })
                .Where(rs => rs.s.ServiceType.HasInterface(interfaceType))
                .Select(rs => rs.s.ServiceType)
                .Where(filter)
                .ToList();
        }
    }
}
