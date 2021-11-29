namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// Extensions for subscribing object instances.
    /// </summary>
    public static class InstanceExtensions
    {
        /// <summary>
        /// Subscribes an object instance to the bus
        /// </summary>
        /// <param name="configurator">
        /// Service Bus Service Configurator
        /// - the item that is passed as a parameter to
        /// the action that is calling the configurator.
        /// </param>
        /// <param name="instance">The instance to subscribe.</param>
        /// <returns>An instance subscription configurator.</returns>
        public static void Instance(this IReceiveEndpointConfigurator configurator, object instance)
        {
            var instanceConfigurator = new InstanceConfigurator(instance);

            configurator.AddEndpointSpecification(instanceConfigurator);
        }

        /// <summary>
        /// Connects any consumers for the object to the message dispatcher
        /// </summary>
        /// <param name="connector">The service bus to configure</param>
        /// <param name="instance"></param>
        /// <returns>
        /// The unsubscribe action that can be called to unsubscribe the instance
        /// passed as an argument.
        /// </returns>
        public static ConnectHandle ConnectInstance(this IConsumePipeConnector connector, object instance)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return InstanceConnectorCache.GetInstanceConnector(instance.GetType()).ConnectInstance(connector, instance);
        }

        /// <summary>
        /// Subscribes an object instance to the bus
        /// </summary>
        /// <param name="configurator">
        /// Service Bus Service Configurator
        /// - the item that is passed as a parameter to
        /// the action that is calling the configurator.
        /// </param>
        /// <param name="instance">The instance to subscribe.</param>
        /// <param name="configure">Configure the instance</param>
        /// <returns>An instance subscription configurator.</returns>
        public static void Instance<T>(this IReceiveEndpointConfigurator configurator, T instance, Action<IInstanceConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var instanceConfigurator = new InstanceConfigurator<T>(instance, configurator);

            configure?.Invoke(instanceConfigurator);

            configurator.AddEndpointSpecification(instanceConfigurator);
        }

        /// <summary>
        /// Connects any consumers for the object to the message dispatcher
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="connector">The service bus instance to call this method on.</param>
        /// <param name="instance">The instance to subscribe.</param>
        /// <returns>
        /// The unsubscribe action that can be called to unsubscribe the instance
        /// passed as an argument.
        /// </returns>
        public static ConnectHandle ConnectInstance<T>(this IConsumePipeConnector connector, T instance)
            where T : class, IConsumer
        {
            return InstanceConnectorCache<T>.Connector.ConnectInstance(connector, instance);
        }
    }
}
