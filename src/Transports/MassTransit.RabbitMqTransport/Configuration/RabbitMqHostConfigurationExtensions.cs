namespace MassTransit
{
    using System;
    using RabbitMqTransport.Configuration;


    public static class RabbitMqHostConfigurationExtensions
    {
        /// <summary>
        /// Configure the RabbitMQ host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the RabbitMQ host (rabbitmq://host:port/vhost)</param>
        /// <param name="configure"></param>
        public static void Host(this IRabbitMqBusFactoryConfigurator configurator, Uri hostAddress,
            Action<IRabbitMqHostConfigurator> configure = null)
        {
            configurator.Host(hostAddress, null, configure);
        }

        /// <summary>
        /// Configure the RabbitMQ host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker, or a well-formed URI host address</param>
        /// <param name="configure">The configuration callback</param>
        public static void Host(this IRabbitMqBusFactoryConfigurator configurator, string host,
            Action<IRabbitMqHostConfigurator> configure = null)
        {
            if (Uri.IsWellFormedUriString(host, UriKind.Absolute))
                configurator.Host(new Uri(host), null, configure);
            else
                configurator.Host(host, "/", null, configure);
        }

        /// <summary>
        /// Configure the RabbitMQ host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the RabbitMQ host (rabbitmq://host:port/vhost)</param>
        /// <param name="connectionName">The client-provided connection name</param>
        /// <param name="configure"></param>
        public static void Host(this IRabbitMqBusFactoryConfigurator configurator, Uri hostAddress, string connectionName,
            Action<IRabbitMqHostConfigurator> configure = null)
        {
            var hostConfigurator = new RabbitMqHostConfigurator(hostAddress, connectionName);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure the RabbitMQ host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="configure">The configuration callback</param>
        public static void Host(this IRabbitMqBusFactoryConfigurator configurator, string host, string virtualHost,
            Action<IRabbitMqHostConfigurator> configure = null)
        {
            configurator.Host(host, virtualHost, null, configure);
        }

        /// <summary>
        /// Configure the RabbitMQ host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="connectionName">The client-provided connection name</param>
        /// <param name="configure">The configuration callback</param>
        public static void Host(this IRabbitMqBusFactoryConfigurator configurator, string host, string virtualHost, string connectionName,
            Action<IRabbitMqHostConfigurator> configure = null)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));
            if (virtualHost == null)
                throw new ArgumentNullException(nameof(virtualHost));

            var hostConfigurator = new RabbitMqHostConfigurator(host, virtualHost, connectionName: connectionName);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure the RabbitMQ host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="configure">The configuration callback</param>
        public static void Host(this IRabbitMqBusFactoryConfigurator configurator, string host, ushort port, string virtualHost,
            Action<IRabbitMqHostConfigurator> configure)
        {
            configurator.Host(host, port, virtualHost, null, configure);
        }

        /// <summary>
        /// Configure the RabbitMQ host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="connectionName">The client-provided connection name</param>
        /// <param name="configure">The configuration callback</param>
        public static void Host(this IRabbitMqBusFactoryConfigurator configurator, string host, ushort port, string virtualHost,
            string connectionName, Action<IRabbitMqHostConfigurator> configure = null)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));
            if (virtualHost == null)
                throw new ArgumentNullException(nameof(virtualHost));

            var hostConfigurator = new RabbitMqHostConfigurator(host, virtualHost, port, connectionName);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IRabbitMqBusFactoryConfigurator configurator, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
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
        public static void ReceiveEndpoint(this IRabbitMqBusFactoryConfigurator configurator, IEndpointDefinition definition,
            Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(definition, DefaultEndpointNameFormatter.Instance, configure);
        }
    }
}
