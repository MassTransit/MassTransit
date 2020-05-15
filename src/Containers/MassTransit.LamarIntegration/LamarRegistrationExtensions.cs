namespace MassTransit
{
    using System;
    using System.Linq;
    using Lamar;
    using LamarIntegration;
    using LamarIntegration.Registration;
    using Metadata;


    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class LamarRegistrationExtensions
    {
        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="configure"></param>
        public static void AddMassTransit(this ServiceRegistry registry, Action<IServiceRegistryConfigurator> configure = null)
        {
            var configurator = new ServiceRegistryRegistrationConfigurator(registry);

            configure?.Invoke(configurator);
        }

        /// <summary>
        /// Add consumers that were already added to the container to the registration
        /// </summary>
        public static void AddConsumersFromContainer(this IRegistrationConfigurator configurator, IContainer container)
        {
            var consumerTypes = container.FindTypes(TypeMetadataCache.IsConsumerOrDefinition);
            configurator.AddConsumers(consumerTypes);
        }

        /// <summary>
        /// Add sagas that were already added to the container to the registration
        /// </summary>
        public static void AddSagasFromContainer(this IRegistrationConfigurator configurator, IContainer container)
        {
            var sagaTypes = container.FindTypes(TypeMetadataCache.IsSagaOrDefinition);
            configurator.AddSagas(sagaTypes);
        }

        static Type[] FindTypes(this IContainer container, Func<Type, bool> filter)
        {
            return container.Model.ServiceTypes
                .Where(rs => filter(rs.ServiceType))
                .Select(rs => rs.ServiceType)
                .Concat(container.Model.AllInstances.Where(x => filter(x.ImplementationType)).Select(x => x.ImplementationType))
                .ToArray();
        }
    }
}
