namespace MassTransit
{
    using System;
    using System.Linq;
    using Mediator;
    using Metadata;
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
        public static Container AddMassTransit(this Container container, Action<ISimpleInjectorBusConfigurator> configure = null)
        {
            if (container.GetCurrentRegistrations().Any(d => d.ServiceType == typeof(IBus)))
            {
                throw new ConfigurationException(
                    "AddBus() was already called. To configure multiple bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            var configurator = new SimpleInjectorBusConfigurator(container);

            configure?.Invoke(configurator);

            return container;
        }

        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        public static Container AddMediator(this Container container, Action<ISimpleInjectorMediatorConfigurator> configure = null)
        {
            if (container.GetCurrentRegistrations().Any(d => d.ServiceType == typeof(IMediator)))
                throw new ConfigurationException("AddMediator() was already called and may only be called once per container.");

            var configurator = new SimpleInjectorMediatorConfigurator(container);

            configure?.Invoke(configurator);

            return container;
        }

        /// <summary>
        /// Add consumers that were already added to the container to the registration
        /// </summary>
        public static void AddConsumersFromContainer(this IRegistrationConfigurator configurator, Container container)
        {
            Type[] consumerTypes = container.FindTypes(TypeMetadataCache.IsConsumerOrDefinition);
            configurator.AddConsumers(consumerTypes);
        }

        /// <summary>
        /// Add sagas that were already added to the container to the registration
        /// </summary>
        public static void AddSagasFromContainer(this IRegistrationConfigurator configurator, Container container)
        {
            Type[] sagaTypes = container.FindTypes(TypeMetadataCache.IsSagaOrDefinition);
            configurator.AddSagas(sagaTypes);
        }

        static Type[] FindTypes(this Container container, Func<Type, bool> filter)
        {
            return container.GetCurrentRegistrations()
                .Where(rs => filter(rs.Registration.ImplementationType))
                .Select(rs => rs.Registration.ImplementationType)
                .ToArray();
        }
    }
}
