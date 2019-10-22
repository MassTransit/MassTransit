namespace MassTransit
{
    using System;
    using Castle.Windsor;
    using ConsumeConfigurators;
    using Definition;
    using Saga;
    using WindsorIntegration;
    using WindsorIntegration.Registration;


    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class WindsorRegistrationExtensions
    {
        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        public static void AddMassTransit(this IWindsorContainer container, Action<IWindsorContainerConfigurator> configure = null)
        {
            var configurator = new WindsorContainerRegistrationConfigurator(container);

            configure?.Invoke(configurator);
        }

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
        public static void ConfigureEndpoints<T>(this T configurator, IWindsorContainer container, IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveConfigurator
        {
            var registration = container.Resolve<IRegistration>();

            registration.ConfigureEndpoints(configurator, endpointNameFormatter);

            container.Release(registration);
        }

        /// <summary>
        /// Configure a consumer (or consumers) on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="consumerTypes">The consumer type(s) to configure</param>
        public static void ConfigureConsumer(this IReceiveEndpointConfigurator configurator, IWindsorContainer container, params Type[] consumerTypes)
        {
            var registration = container.Resolve<IRegistration>();

            foreach (var consumerType in consumerTypes)
                registration.ConfigureConsumer(consumerType, configurator);

            container.Release(registration);
        }

        /// <summary>
        /// Configure a consumer on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        public static void ConfigureConsumer<T>(this IReceiveEndpointConfigurator configurator, IWindsorContainer container,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var registration = container.Resolve<IRegistration>();

            registration.ConfigureConsumer(configurator, configure);

            container.Release(registration);
        }

        /// <summary>
        /// Configure all registered consumers on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void ConfigureConsumers(this IReceiveEndpointConfigurator configurator, IWindsorContainer container)
        {
            var registration = container.Resolve<IRegistration>();

            registration.ConfigureConsumers(configurator);

            container.Release(registration);
        }

        /// <summary>
        /// Configure a saga (or sagas) on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="sagaTypes">The saga type(s) to configure</param>
        public static void ConfigureSaga(this IReceiveEndpointConfigurator configurator, IWindsorContainer container, params Type[] sagaTypes)
        {
            var registration = container.Resolve<IRegistration>();

            foreach (var sagaType in sagaTypes)
                registration.ConfigureSaga(sagaType, configurator);

            container.Release(registration);
        }

        /// <summary>
        /// Configure a saga on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        public static void ConfigureSaga<T>(this IReceiveEndpointConfigurator configurator, IWindsorContainer container,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var registration = container.Resolve<IRegistration>();

            registration.ConfigureSaga(configurator, configure);

            container.Release(registration);
        }

        /// <summary>
        /// Configure all registered sagas on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void ConfigureSagas(this IReceiveEndpointConfigurator configurator, IWindsorContainer container)
        {
            var registration = container.Resolve<IRegistration>();

            registration.ConfigureSagas(configurator);

            container.Release(registration);
        }

        /// <summary>
        /// Configure the execute activity on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="activityType"></param>
        public static void ConfigureExecuteActivity(this IReceiveEndpointConfigurator configurator, IWindsorContainer container, Type activityType)
        {
            var registration = container.Resolve<IRegistration>();

            registration.ConfigureExecuteActivity(activityType, configurator);

            container.Release(registration);
        }

        /// <summary>
        /// Configure an activity on two endpoints, one for execute, and the other for compensate
        /// </summary>
        /// <param name="executeEndpointConfigurator"></param>
        /// <param name="compensateEndpointConfigurator"></param>
        /// <param name="container"></param>
        /// <param name="activityType"></param>
        public static void ConfigureActivity(this IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator, IWindsorContainer container, Type activityType)
        {
            var registration = container.Resolve<IRegistration>();

            registration.ConfigureActivity(activityType, executeEndpointConfigurator, compensateEndpointConfigurator);

            container.Release(registration);
        }
    }
}
