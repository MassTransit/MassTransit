// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using Definition;
    using RabbitMqTransport;
    using RabbitMqTransport.Configurators;


    public static class RabbitMqHostConfigurationExtensions
    {
        /// <summary>
        ///     Configure a RabbitMQ host using the configuration API
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the RabbitMQ host (rabbitmq://host:port/vhost)</param>
        /// <param name="configure"></param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, Uri hostAddress,
            Action<IRabbitMqHostConfigurator> configure)
        {
            return configurator.Host(hostAddress, null, configure);
        }

        /// <summary>
        ///     Configure a RabbitMQ host using the configuration API
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the RabbitMQ host (rabbitmq://host:port/vhost)</param>
        /// <param name="connectionName">The client-provided connection name</param>
        /// <param name="configure"></param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, Uri hostAddress, string connectionName,
            Action<IRabbitMqHostConfigurator> configure = null)
        {
            var hostConfigurator = new RabbitMqHostConfigurator(hostAddress, connectionName);

            configure?.Invoke(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure a RabbitMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="configure">The configuration callback</param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, string host, string virtualHost,
            Action<IRabbitMqHostConfigurator> configure = null)
        {
            return configurator.Host(host, virtualHost, null, configure);
        }

        /// <summary>
        /// Configure a RabbitMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="configure">The configuration callback</param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, string host, Action<IRabbitMqHostConfigurator> configure = null)
        {
            if (Uri.IsWellFormedUriString(host, UriKind.Absolute))
                return configurator.Host(new Uri(host), null, configure);

            return configurator.Host(host, "/", null, configure);
        }

        /// <summary>
        /// Configure a RabbitMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="connectionName">The client-provided connection name</param>
        /// <param name="configure">The configuration callback</param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, string host, string virtualHost,
            string connectionName, Action<IRabbitMqHostConfigurator> configure = null)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (virtualHost == null)
                throw new ArgumentNullException(nameof(virtualHost));

            var hostConfigurator = new RabbitMqHostConfigurator(host, virtualHost, connectionName: connectionName);

            configure?.Invoke(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure a RabbitMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="configure">The configuration callback</param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, string host, ushort port, string virtualHost,
            Action<IRabbitMqHostConfigurator> configure)
        {
            return configurator.Host(host, port, virtualHost, null, configure);
        }

        /// <summary>
        /// Configure a RabbitMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host name of the broker</param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="connectionName">The client-provided connection name</param>
        /// <param name="configure">The configuration callback</param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, string host, ushort port, string virtualHost,
            string connectionName, Action<IRabbitMqHostConfigurator> configure = null)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (virtualHost == null)
                throw new ArgumentNullException(nameof(virtualHost));

            var hostConfigurator = new RabbitMqHostConfigurator(host, virtualHost, port, connectionName);

            configure?.Invoke(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        [Obsolete("The host parameter is no longer required, and can be removed")]
        public static void ReceiveEndpoint(this IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host,
            Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(host, new TemporaryEndpointDefinition(), DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host"></param>
        /// <param name="definition"></param>
        /// <param name="configure"></param>
        [Obsolete("The host parameter is no longer required, and can be removed")]
        public static void ReceiveEndpoint(this IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host, IEndpointDefinition definition,
            Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(host, definition, DefaultEndpointNameFormatter.Instance, configure);
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
