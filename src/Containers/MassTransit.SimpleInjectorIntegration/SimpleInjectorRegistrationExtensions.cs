namespace MassTransit
{
    using System;
    using System.Linq;
    using Conductor.Configuration;
    using ConsumeConfigurators;
    using Definition;
    using Metadata;
    using Saga;
    using SimpleInjector;
    using SimpleInjectorIntegration;
    using SimpleInjectorIntegration.Registration;


    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class SimpleInjectorRegistrationExtensions
    {
        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        public static Container AddMassTransit(this Container container, Action<ISimpleInjectorConfigurator> configure = null)
        {
            var configurator = new SimpleInjectorRegistrationConfigurator(container);

            configure?.Invoke(configurator);

            return container;
        }

        /// <summary>
        /// Add consumers that were already added to the container to the registration
        /// </summary>
        public static void AddConsumersFromContainer(this IRegistrationConfigurator configurator, Container container)
        {
            var consumerTypes = container.FindTypes(TypeMetadataCache.IsConsumerOrDefinition);
            configurator.AddConsumers(consumerTypes);
        }

        /// <summary>
        /// Add sagas that were already added to the container to the registration
        /// </summary>
        public static void AddSagasFromContainer(this IRegistrationConfigurator configurator, Container container)
        {
            var sagaTypes = container.FindTypes(TypeMetadataCache.IsSagaOrDefinition);
            configurator.AddSagas(sagaTypes);
        }

        static Type[] FindTypes(this Container container, Func<Type, bool> filter)
        {
            return container.GetCurrentRegistrations()
                .Where(rs => filter(rs.Registration.ImplementationType))
                .Select(rs => rs.Registration.ImplementationType)
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
        /// <param name="container">The container reference</param>
        /// <param name="endpointNameFormatter">Optional, the endpoint name formatter</param>
        /// <typeparam name="T">The bus factory type (depends upon the transport)</typeparam>
        public static void ConfigureEndpoints<T>(this IReceiveConfigurator<T> configurator, Container container,
            IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveEndpointConfigurator
        {
            var registration = container.GetInstance<IRegistration>();

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
        /// <param name="container">The container reference</param>
        /// <param name="options">The service instance options</param>
        /// <typeparam name="TEndpointConfigurator">The ReceiveEndpointConfigurator type for the transport</typeparam>
        public static void ConfigureServiceEndpoints<TEndpointConfigurator>(this IReceiveConfigurator<TEndpointConfigurator> configurator,
            Container container, ServiceInstanceOptions options = null)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            var registration = container.GetInstance<IRegistration>();

            options ??= new ServiceInstanceOptions();
            if (options.EndpointNameFormatter is DefaultEndpointNameFormatter)
            {
                var formatter = container.TryGetInstance<IEndpointNameFormatter>();
                if (formatter != null)
                    options.SetEndpointNameFormatter(formatter);
            }

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
            IRegistrationContext<Container> registration, ServiceInstanceOptions options = null)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            options ??= new ServiceInstanceOptions();
            if (options.EndpointNameFormatter is DefaultEndpointNameFormatter)
            {
                var formatter = registration.Container.TryGetInstance<IEndpointNameFormatter>();
                if (formatter != null)
                    options.SetEndpointNameFormatter(formatter);
            }

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
        /// <param name="container"></param>
        /// <param name="consumerTypes">The consumer type(s) to configure</param>
        public static void ConfigureConsumer(this IReceiveEndpointConfigurator configurator, Container container, params Type[] consumerTypes)
        {
            var registration = container.GetInstance<IRegistration>();

            foreach (var consumerType in consumerTypes)
                registration.ConfigureConsumer(consumerType, configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure a consumer on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        public static void ConfigureConsumer<T>(this IReceiveEndpointConfigurator configurator, Container container,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var registration = container.GetInstance<IRegistration>();

            registration.ConfigureConsumer(configurator, configure);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure all registered consumers on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void ConfigureConsumers(this IReceiveEndpointConfigurator configurator, Container container)
        {
            var registration = container.GetInstance<IRegistration>();

            registration.ConfigureConsumers(configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure a saga (or sagas) on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="sagaTypes">The saga type(s) to configure</param>
        public static void ConfigureSaga(this IReceiveEndpointConfigurator configurator, Container container, params Type[] sagaTypes)
        {
            var registration = container.GetInstance<IRegistration>();

            foreach (var sagaType in sagaTypes)
                registration.ConfigureSaga(sagaType, configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure a saga on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        public static void ConfigureSaga<T>(this IReceiveEndpointConfigurator configurator, Container container,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var registration = container.GetInstance<IRegistration>();

            registration.ConfigureSaga(configurator, configure);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure all registered sagas on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void ConfigureSagas(this IReceiveEndpointConfigurator configurator, Container container)
        {
            var registration = container.GetInstance<IRegistration>();

            registration.ConfigureSagas(configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure the execute activity on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="activityType"></param>
        public static void ConfigureExecuteActivity(this IReceiveEndpointConfigurator configurator, Container container, Type activityType)
        {
            var registration = container.GetInstance<IRegistration>();

            registration.ConfigureExecuteActivity(activityType, configurator);
        }

        [Obsolete("Please use IRegistrationContext instead")]
        /// <summary>
        /// Configure an activity on two endpoints, one for execute, and the other for compensate
        /// </summary>
        /// <param name="executeEndpointConfigurator"></param>
        /// <param name="compensateEndpointConfigurator"></param>
        /// <param name="container"></param>
        /// <param name="activityType"></param>
        public static void ConfigureActivity(this IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator, Container container, Type activityType)
        {
            var registration = container.GetInstance<IRegistration>();

            registration.ConfigureActivity(activityType, executeEndpointConfigurator, compensateEndpointConfigurator);
        }
    }
}
