namespace MassTransit
{
    using System;
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using AutofacIntegration;
    using AutofacIntegration.Registration;
    using Mediator;
    using Metadata;


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
        public static ContainerBuilder AddMassTransit(this ContainerBuilder builder, Action<IContainerBuilderBusConfigurator> configure = null)
        {
            if (builder.ComponentRegistryBuilder.IsRegistered(new TypedService(typeof(IRegistration))))
            {
                throw new ConfigurationException(
                    "AddBus() was already called. To configure multiple bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            var configurator = new ContainerBuilderBusRegistrationConfigurator(builder);

            configure?.Invoke(configurator);

            return builder;
        }

        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        public static ContainerBuilder AddMediator(this ContainerBuilder builder, Action<IContainerBuilderMediatorConfigurator> configure = null)
        {
            if (builder.ComponentRegistryBuilder.IsRegistered(new TypedService(typeof(IMediator))))
                throw new ConfigurationException("AddMediator() was already called and may only be called once per container.");

            var configurator = new ContainerBuilderMediatorConfigurator(builder);

            configure?.Invoke(configurator);

            return builder;
        }

        /// <summary>
        /// Add consumers that were already added to the container to the registration
        /// </summary>
        public static void AddConsumersFromContainer(this IRegistrationConfigurator configurator, IComponentContext context)
        {
            Type[] consumerTypes = context.FindTypes(TypeMetadataCache.IsConsumerOrDefinition);
            configurator.AddConsumers(consumerTypes);
        }

        /// <summary>
        /// Add sagas that were already added to the container to the registration
        /// </summary>
        public static void AddSagasFromContainer(this IRegistrationConfigurator configurator, IComponentContext context)
        {
            Type[] sagaTypes = context.FindTypes(TypeMetadataCache.IsSagaOrDefinition);
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
    }
}
