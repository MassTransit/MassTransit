namespace MassTransit
{
    using System;
    using Autofac;
    using AutofacIntegration;
    using AutofacIntegration.Registration;
    using ConsumeConfigurators;
    using Definition;
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
        public static void AddMassTransit(this ContainerBuilder builder, Action<IContainerBuilderConfigurator> configure = null)
        {
            var configurator = new ContainerBuilderRegistrationConfigurator(builder);

            configure?.Invoke(configurator);
        }

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
        public static void ConfigureEndpoints<T>(this T configurator, IComponentContext context, IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveConfigurator
        {
            var registration = context.Resolve<IRegistration>();

            registration.ConfigureEndpoints(configurator, endpointNameFormatter);
        }

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
