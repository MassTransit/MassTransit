namespace MassTransit
{
    using System;


    public static class ReceiveEndpointConfigurationExtensions
    {
        /// <summary>
        /// Creates a temporary endpoint, with a dynamically generated name, that should be removed when the bus is stopped.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void ReceiveEndpoint(this IBusFactoryConfigurator configurator, Action<IReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(new TemporaryEndpointDefinition(), DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void ReceiveEndpoint(this IBusFactoryConfigurator configurator, IEndpointDefinition definition, Action<IReceiveEndpointConfigurator>
            configure = null)
        {
            configurator.ReceiveEndpoint(definition, DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void ManagementEndpoint(this IBusFactoryConfigurator configurator, Action<IReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(new ManagementEndpointDefinition(), DefaultEndpointNameFormatter.Instance, x =>
            {
                configure?.Invoke(x);
            });
        }
    }
}
