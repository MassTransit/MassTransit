namespace MassTransit
{
    using System;
    using System.Linq;
    using Castle.MicroKernel;
    using Castle.Windsor;
    using Conductor.Configuration;
    using ConsumeConfigurators;
    using Definition;
    using Metadata;
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
        public static IWindsorContainer AddMassTransit(this IWindsorContainer container, Action<IWindsorContainerConfigurator> configure = null)
        {
            var configurator = new WindsorContainerRegistrationConfigurator(container);

            configure?.Invoke(configurator);

            return container;
        }

        /// <summary>
        /// Add consumers that were already added to the container to the registration
        /// </summary>
        public static void AddConsumersFromContainer(this IRegistrationConfigurator configurator, IWindsorContainer container)
        {
            var consumerTypes = container.FindTypes(TypeMetadataCache.IsConsumerOrDefinition);
            configurator.AddConsumers(consumerTypes);
        }

        /// <summary>
        /// Add sagas that were already added to the container to the registration
        /// </summary>
        public static void AddSagasFromContainer(this IRegistrationConfigurator configurator, IWindsorContainer container)
        {
            var sagaTypes = container.FindTypes(TypeMetadataCache.IsSagaOrDefinition);
            configurator.AddSagas(sagaTypes);
        }

        static Type[] FindTypes(this IWindsorContainer container, Func<Type, bool> filter)
        {
            return container.Kernel.GetHandlers()
                .Where(rs => rs.ComponentModel.Services.Any(filter))
                .Select(rs => rs.ComponentModel.Implementation)
                .ToArray();
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
            IRegistrationContext<IKernel> registration, ServiceInstanceOptions options = null)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            options ??= new ServiceInstanceOptions();
            if (options.EndpointNameFormatter is DefaultEndpointNameFormatter && registration.Container.HasComponent(typeof(IEndpointNameFormatter)))
                options.SetEndpointNameFormatter(registration.Container.Resolve<IEndpointNameFormatter>());

            configurator.ServiceInstance(options, instanceConfigurator =>
            {
                registration.ConfigureEndpoints(instanceConfigurator, instanceConfigurator.EndpointNameFormatter);
            });
        }
    }
}
