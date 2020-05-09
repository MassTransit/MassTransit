namespace MassTransit
{
    using System;
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using AutofacIntegration;
    using AutofacIntegration.Registration;
    using Conductor.Configuration;
    using ConsumeConfigurators;
    using Definition;
    using Metadata;
    using Saga;


    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class AutofacRegistrationExtensions
    {
        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        public static ContainerBuilder AddMassTransit(this ContainerBuilder builder, Action<IContainerBuilderConfigurator> configure = null)
        {
            var configurator = new ContainerBuilderRegistrationConfigurator(builder);

            configure?.Invoke(configurator);

            return builder;
        }

        /// <summary>
        /// Add consumers that were already added to the container to the registration
        /// </summary>
        public static void AddConsumersFromContainer(this IRegistrationConfigurator configurator, IComponentContext context)
        {
            var consumerTypes = context.FindTypes(TypeMetadataCache.IsConsumerOrDefinition);
            configurator.AddConsumers(consumerTypes);
        }

        /// <summary>
        /// Add sagas that were already added to the container to the registration
        /// </summary>
        public static void AddSagasFromContainer(this IRegistrationConfigurator configurator, IComponentContext context)
        {
            var sagaTypes = context.FindTypes(TypeMetadataCache.IsSagaOrDefinition);
            configurator.AddSagas(sagaTypes);
        }

        static Type[] FindTypes(this IComponentContext context, Func<Type, bool> filter)
        {
            return context.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => (r, s))
                .Where(rs => filter(rs.s.ServiceType))
                .Select(rs => rs.s.ServiceType)
                .ToArray();
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure the endpoints for all defined consumer, saga, and activity types using an optional
        /// endpoint name formatter. If no endpoint name formatter is specified and an <see cref="IEndpointNameFormatter"/>
        /// is registered in the container, it is resolved from the container. Otherwise, the <see cref="DefaultEndpointNameFormatter"/>
        /// is used.
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator"/> for the bus being configured</param>
        /// <param name="context">The container reference</param>
        /// <param name="endpointNameFormatter">Optional, the endpoint name formatter</param>
        /// <typeparam name="T">The bus factory type (depends upon the transport)</typeparam>
        public static void ConfigureEndpoints<T>(this IReceiveConfigurator<T> configurator, IComponentContext context,
            IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveEndpointConfigurator
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureEndpoints(configurator, endpointNameFormatter);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure service endpoints for all defined consumer, saga, and activity types using an optional
        /// endpoint name formatter. If no endpoint name formatter is specified, and an <see cref="IEndpointNameFormatter"/>
        /// is registered in the container, it is resolved from the container. Otherwise, the <see cref="DefaultEndpointNameFormatter"/>
        /// is used.
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator"/> for the bus being configured</param>
        /// <param name="context">The container reference</param>
        /// <param name="options">The service instance options</param>
        /// <typeparam name="TEndpointConfigurator">The ReceiveEndpointConfigurator type for the transport</typeparam>
        public static void ConfigureServiceEndpoints<TEndpointConfigurator>(this IReceiveConfigurator<TEndpointConfigurator> configurator,
            IComponentContext context, ServiceInstanceOptions options = null)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            var registration = context.Resolve<IRegistration>();

            options ??= new ServiceInstanceOptions();
            if (options.EndpointNameFormatter is DefaultEndpointNameFormatter && context.TryResolve<IEndpointNameFormatter>(out var formatter))
                options.SetEndpointNameFormatter(formatter);

            configurator.ServiceInstance(options, instanceConfigurator =>
            {
                registration.ConfigureEndpoints(instanceConfigurator, instanceConfigurator.EndpointNameFormatter);
            });
        }

        /// <summary>
        /// Configure service endpoints for all defined consumer, saga, and activity types using an optional
        /// endpoint name formatter. If no endpoint name formatter is specified, and an <see cref="IEndpointNameFormatter"/>
        /// is registered in the container, it is resolved from the container. Otherwise, the <see cref="DefaultEndpointNameFormatter"/>
        /// is used.
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator"/> for the bus being configured</param>
        /// <param name="registration">The container registration</param>
        /// <param name="options">The service instance options</param>
        /// <typeparam name="TEndpointConfigurator">The ReceiveEndpointConfigurator type for the transport</typeparam>
        public static void ConfigureServiceEndpoints<TEndpointConfigurator>(this IReceiveConfigurator<TEndpointConfigurator> configurator,
            IRegistrationContext<IComponentContext> registration, ServiceInstanceOptions options = null)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            options ??= new ServiceInstanceOptions();
            if (options.EndpointNameFormatter is DefaultEndpointNameFormatter && registration.Container.TryResolve<IEndpointNameFormatter>(out var formatter))
                options.SetEndpointNameFormatter(formatter);

            configurator.ServiceInstance(options, instanceConfigurator =>
            {
                registration.ConfigureEndpoints(instanceConfigurator, instanceConfigurator.EndpointNameFormatter);
            });
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure a consumer (or consumers) on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="consumerTypes">The consumer type(s) to configure</param>
        public static void ConfigureConsumer(this IReceiveEndpointConfigurator configurator, IComponentContext context, params Type[] consumerTypes)
        {
            var registration = context.Resolve<IRegistration>();

            foreach (var consumerType in consumerTypes)
                registration.ConfigureConsumer(consumerType, configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure a consumer on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        public static void ConfigureConsumer<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureConsumer(configurator, configure);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure all registered consumers on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        public static void ConfigureConsumers(this IReceiveEndpointConfigurator configurator, IComponentContext context)
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureConsumers(configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure a saga (or sagas) on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="sagaTypes">The saga type(s) to configure</param>
        public static void ConfigureSaga(this IReceiveEndpointConfigurator configurator, IComponentContext context, params Type[] sagaTypes)
        {
            var registration = context.Resolve<IRegistration>();

            foreach (var sagaType in sagaTypes)
                registration.ConfigureSaga(sagaType, configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure a saga on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        public static void ConfigureSaga<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureSaga(configurator, configure);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure all registered sagas on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        public static void ConfigureSagas(this IReceiveEndpointConfigurator configurator, IComponentContext context)
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureSagas(configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure the execute activity on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="activityType"></param>
        public static void ConfigureExecuteActivity(this IReceiveEndpointConfigurator configurator, IComponentContext context, Type activityType)
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureExecuteActivity(activityType, configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure an activity on two endpoints, one for execute, and the other for compensate
        /// </summary>
        /// <param name="executeEndpointConfigurator"></param>
        /// <param name="compensateEndpointConfigurator"></param>
        /// <param name="context"></param>
        /// <param name="activityType"></param>
        public static void ConfigureActivity(this IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator, IComponentContext context, Type activityType)
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureActivity(activityType, executeEndpointConfigurator, compensateEndpointConfigurator);
        }
    }
}
