namespace MassTransit
{
    using System;
    using ActiveMqTransport;
    using ActiveMqTransport.Configuration;


    public static class ActiveMqHostConfigurationExtensions
    {
        /// <summary>
        /// Configure a ActiveMQ host using the configuration API
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the ActiveMQ host (activemq://host:port/vhost)</param>
        /// <param name="configure"></param>
        public static void Host(this IActiveMqBusFactoryConfigurator configurator, Uri hostAddress, Action<IActiveMqHostConfigurator> configure)
        {
            if (hostAddress == null)
                throw new ArgumentNullException(nameof(hostAddress));

            var hostConfigurator = new ActiveMqHostConfigurator(hostAddress);

            configure(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure a ActiveMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostName">The host name of the broker</param>
        /// <param name="configure">The configuration callback</param>
        public static void Host(this IActiveMqBusFactoryConfigurator configurator, string hostName, Action<IActiveMqHostConfigurator> configure)
        {
            if (Uri.IsWellFormedUriString(hostName, UriKind.Absolute))
                configurator.Host(new Uri(hostName), configure);
            else
                configurator.Host(new ActiveMqHostAddress(hostName, default, "/"), configure);
        }

        /// <summary>
        /// Configure a ActiveMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostName">The host name of the broker</param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="configure">The configuration callback</param>
        public static void Host(this IActiveMqBusFactoryConfigurator configurator, string hostName, int port,
            Action<IActiveMqHostConfigurator> configure)
        {
            configurator.Host(new ActiveMqHostAddress(hostName, port, "/"), configure);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IActiveMqBusFactoryConfigurator configurator,
            Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(new TemporaryEndpointDefinition(), DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IActiveMqBusFactoryConfigurator configurator, IEndpointDefinition definition,
            Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(definition, DefaultEndpointNameFormatter.Instance, configure);
        }
    }
}
