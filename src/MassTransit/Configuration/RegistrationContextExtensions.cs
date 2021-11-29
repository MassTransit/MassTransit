namespace MassTransit
{
    using System;
    using DependencyInjection.Registration;


    public static class RegistrationContextExtensions
    {
        /// <summary>
        /// Configure the endpoints for all defined consumer, saga, and activity types using an optional
        /// endpoint name formatter. If no endpoint name formatter is specified and an <see cref="IEndpointNameFormatter" />
        /// is registered in the container, it is resolved from the container. Otherwise, the <see cref="DefaultEndpointNameFormatter" />
        /// is used.
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator" /> for the bus being configured</param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="endpointNameFormatter">Optional, the endpoint name formatter</param>
        /// <typeparam name="T">The bus factory type (depends upon the transport)</typeparam>
        public static void ConfigureEndpoints<T>(this IReceiveConfigurator<T> configurator, IBusRegistrationContext registration,
            IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveEndpointConfigurator
        {
            registration.ConfigureEndpoints(configurator, endpointNameFormatter);
        }

        /// <summary>
        /// Configure the endpoints for all defined consumer, saga, and activity types using an optional
        /// endpoint name formatter. If no endpoint name formatter is specified and an <see cref="IEndpointNameFormatter" />
        /// is registered in the container, it is resolved from the container. Otherwise, the <see cref="DefaultEndpointNameFormatter" />
        /// is used.
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator" /> for the bus being configured</param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="configureFilter">Filter the configured consumers, sagas, and activities</param>
        /// <param name="endpointNameFormatter">Optional, the endpoint name formatter</param>
        /// <typeparam name="T">The bus factory type (depends upon the transport)</typeparam>
        public static void ConfigureEndpoints<T>(this IReceiveConfigurator<T> configurator, IBusRegistrationContext registration,
            Action<IRegistrationFilterConfigurator> configureFilter, IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveEndpointConfigurator
        {
            registration.ConfigureEndpoints(configurator, endpointNameFormatter, configureFilter);
        }

        /// <summary>
        /// Configure a service instance for use with the job service
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator" /> for the bus being configured</param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="configureFilter">Filter the configured consumers, sagas, and activities</param>
        /// <param name="options">Optional service instance options to start</param>
        /// <typeparam name="T">The bus factory type (depends upon the transport)</typeparam>
        public static void ConfigureServiceEndpoints<T>(this IBusFactoryConfigurator<T> configurator, IBusRegistrationContext registration,
            Action<IRegistrationFilterConfigurator> configureFilter, ServiceInstanceOptions options = null)
            where T : IReceiveEndpointConfigurator
        {
            options ??= new ServiceInstanceOptions();
            if (options.EndpointNameFormatter is DefaultEndpointNameFormatter)
            {
                var formatter = registration.EndpointNameFormatter;
                if (formatter != null)
                    options.SetEndpointNameFormatter(formatter);
            }

            configurator.ServiceInstance(options, instanceConfigurator =>
            {
                if (options.TryGetOptions(out JobServiceOptions jobServiceOptions))
                    instanceConfigurator.ConfigureJobServiceEndpoints(jobServiceOptions);

                registration.ConfigureEndpoints(instanceConfigurator, instanceConfigurator.EndpointNameFormatter, configureFilter);
            });
        }

        /// <summary>
        /// Configure a service instance for use with the job service
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator" /> for the bus being configured</param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="options">Optional service instance options to start</param>
        /// <typeparam name="T">The bus factory type (depends upon the transport)</typeparam>
        public static void ConfigureServiceEndpoints<T>(this IBusFactoryConfigurator<T> configurator, IBusRegistrationContext registration,
            ServiceInstanceOptions options = null)
            where T : IReceiveEndpointConfigurator
        {
            ConfigureServiceEndpoints(configurator, registration, null, options);
        }

        /// <summary>
        /// Configure a consumer on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="consumerType">The consumer type</param>
        public static void ConfigureConsumer(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type consumerType)
        {
            registration.ConfigureConsumer(consumerType, configurator);
        }

        /// <summary>
        /// Configure a consumer on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        public static void ConfigureConsumer<T>(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            registration.ConfigureConsumer(configurator, configure);
        }

        /// <summary>
        /// Configure all registered consumers on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registration">The registration for this bus instance</param>
        public static void ConfigureConsumers(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration)
        {
            registration.ConfigureConsumers(configurator);
        }

        /// <summary>
        /// Configure a saga on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="sagaType">The saga type</param>
        public static void ConfigureSaga(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type sagaType)
        {
            registration.ConfigureSaga(sagaType, configurator);
        }

        /// <summary>
        /// Configure a saga on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        public static void ConfigureSaga<T>(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            registration.ConfigureSaga(configurator, configure);
        }

        /// <summary>
        /// Configure all registered sagas on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registration">The registration for this bus instance</param>
        public static void ConfigureSagas(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration)
        {
            registration.ConfigureSagas(configurator);
        }

        /// <summary>
        /// Configure the specified activity type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="compensateEndpointConfigurator">The configurator for the compensate activity endpoint</param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="activityType"></param>
        public static void ConfigureActivity(this IReceiveEndpointConfigurator configurator, IReceiveEndpointConfigurator compensateEndpointConfigurator,
            IRegistrationContext registration,
            Type activityType)
        {
            registration.ConfigureActivity(activityType, configurator, compensateEndpointConfigurator);
        }

        /// <summary>
        /// Configure the specified execute activity type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="activityType"></param>
        public static void ConfigureExecuteActivity(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type activityType)
        {
            registration.ConfigureExecuteActivity(activityType, configurator);
        }

        /// <summary>
        /// Configure the specified activity type
        /// </summary>
        /// <param name="configurator">The configurator for the execute activity endpoint</param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="activityType"></param>
        /// <param name="compensateAddress"></param>
        public static void ConfigureActivityExecute(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type activityType,
            Uri compensateAddress)
        {
            registration.ConfigureActivityExecute(activityType, configurator, compensateAddress);
        }

        /// <summary>
        /// Configure the specified activity type
        /// </summary>
        /// <param name="configurator">The configurator for the compensate activity endpoint</param>
        /// <param name="registration">The registration for this bus instance</param>
        /// <param name="activityType"></param>
        public static void ConfigureActivityCompensate(this IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type activityType)
        {
            registration.ConfigureActivityCompensate(activityType, configurator);
        }
    }
}
