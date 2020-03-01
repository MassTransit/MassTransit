namespace MassTransit
{
    using System;
    using Definition;


    public static class ReceiveEndpointConfigurationExtensions
    {
        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
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
            IReceiveEndpointConfigurator specification = null;
            configurator.ReceiveEndpoint(new ManagementEndpointDefinition(), DefaultEndpointNameFormatter.Instance, x =>
            {
                specification = x;

                configure?.Invoke(specification);
            });
        }

        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void ManagementEndpoint(this IHost host, Action<IReceiveEndpointConfigurator> configure = null)
        {
            IReceiveEndpointConfigurator specification = null;
            host.ConnectReceiveEndpoint(new ManagementEndpointDefinition(), DefaultEndpointNameFormatter.Instance, x =>
            {
                specification = x;

                configure?.Invoke(specification);
            });
        }
    }
}
