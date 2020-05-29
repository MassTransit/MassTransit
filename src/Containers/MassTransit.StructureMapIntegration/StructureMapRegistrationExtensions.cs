namespace MassTransit
{
    using System;
    using System.Linq;
    using Metadata;
    using StructureMap;
    using StructureMapIntegration;
    using StructureMapIntegration.Registration;


    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class StructureMapRegistrationExtensions
    {
        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="configure"></param>
        public static void AddMassTransit(this ConfigurationExpression expression, Action<IConfigurationExpressionConfigurator> configure = null)
        {
            var configurator = new ConfigurationExpressionConfigurator(expression);

            configure?.Invoke(configurator);
        }

        /// <summary>
        /// Adds the required services to the service collection, and allows consumers to be added and/or discovered
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="configure"></param>
        public static void AddMediator(this ConfigurationExpression expression, Action<IConfigurationExpressionMediatorConfigurator> configure = null)
        {
            var configurator = new ConfigurationExpressionMediatorConfigurator(expression);

            configure?.Invoke(configurator);
        }

        /// <summary>
        /// Add consumers that were already added to the container to the registration
        /// </summary>
        public static void AddConsumersFromContainer(this IRegistrationConfigurator configurator, IContainer container)
        {
            Type[] consumerTypes = container.FindTypes(TypeMetadataCache.IsConsumerOrDefinition);
            configurator.AddConsumers(consumerTypes);
        }

        /// <summary>
        /// Add sagas that were already added to the container to the registration
        /// </summary>
        public static void AddSagasFromContainer(this IRegistrationConfigurator configurator, IContainer container)
        {
            Type[] sagaTypes = container.FindTypes(TypeMetadataCache.IsSagaOrDefinition);
            configurator.AddSagas(sagaTypes);
        }

        static Type[] FindTypes(this IContainer container, Func<Type, bool> filter)
        {
            return container.Model.PluginTypes
                .Where(rs => filter(rs.PluginType))
                .Select(rs => rs.PluginType)
                .Concat(container.Model.AllInstances.Where(x => filter(x.ReturnedType)).Select(x => x.ReturnedType))
                .ToArray();
        }
    }
}
