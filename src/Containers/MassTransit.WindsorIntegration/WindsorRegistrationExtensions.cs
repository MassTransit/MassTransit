namespace MassTransit
{
    using System;
    using System.Linq;
    using Castle.Windsor;
    using Metadata;
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
    }
}
