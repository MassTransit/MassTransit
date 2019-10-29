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
namespace MassTransit.ActiveMqTransport
{
    using System;
    using Configurators;
    using Definition;


    public static class ActiveMqHostConfigurationExtensions
    {
        /// <summary>
        ///     Configure a ActiveMQ host using the configuration API
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the ActiveMQ host (activemq://host:port/vhost)</param>
        /// <param name="configure"></param>
        public static IActiveMqHost Host(this IActiveMqBusFactoryConfigurator configurator, Uri hostAddress, Action<IActiveMqHostConfigurator> configure)
        {
            if (hostAddress == null)
                throw new ArgumentNullException(nameof(hostAddress));

            var hostConfigurator = new ActiveMqHostConfigurator(hostAddress);

            configure(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure a ActiveMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostName">The host name of the broker</param>
        /// <param name="configure">The configuration callback</param>
        public static IActiveMqHost Host(this IActiveMqBusFactoryConfigurator configurator, string hostName, Action<IActiveMqHostConfigurator> configure)
        {
            if (Uri.IsWellFormedUriString(hostName, UriKind.Absolute))
                return configurator.Host(new Uri(hostName), configure);

            return configurator.Host(new ActiveMqHostAddress(hostName, default, "/"), configure);
        }

        /// <summary>
        /// Configure a ActiveMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostName">The host name of the broker</param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="configure">The configuration callback</param>
        public static IActiveMqHost Host(this IActiveMqBusFactoryConfigurator configurator, string hostName, int port,
            Action<IActiveMqHostConfigurator> configure)
        {
            return configurator.Host(new ActiveMqHostAddress(hostName, port, "/"), configure);
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
        public static void ReceiveEndpoint(this IActiveMqBusFactoryConfigurator configurator, IActiveMqHost host,
            Action<IActiveMqReceiveEndpointConfigurator> configure = null)
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
        public static void ReceiveEndpoint(this IActiveMqBusFactoryConfigurator configurator, IActiveMqHost host, IEndpointDefinition definition,
            Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(host, definition, DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host"></param>
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
        /// <param name="host"></param>
        /// <param name="definition"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IActiveMqBusFactoryConfigurator configurator, IEndpointDefinition definition,
            Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(definition, DefaultEndpointNameFormatter.Instance, configure);
        }
    }
}
