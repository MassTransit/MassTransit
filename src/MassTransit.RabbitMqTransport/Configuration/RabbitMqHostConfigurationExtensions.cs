// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using RabbitMqTransport;
    using RabbitMqTransport.Configuration;
    using RabbitMqTransport.Configuration.Configurators;


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
            var hostConfigurator = new RabbitMqHostConfigurator(hostAddress);

            configure(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure a RabbitMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="configure">The configuratino callback</param>
        /// <param name="host">The host name of the broker</param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, string host, string virtualHost,
            Action<IRabbitMqHostConfigurator> configure)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));
            if (virtualHost == null)
                throw new ArgumentNullException(nameof(virtualHost));

            var hostConfigurator = new RabbitMqHostConfigurator(host, virtualHost);

            configure(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configure a RabbitMQ host with a host name and virtual host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="port">The port to connect to the broker</param>
        /// <param name="virtualHost">The virtual host to use</param>
        /// <param name="configure">The configuratino callback</param>
        /// <param name="host">The host name of the broker</param>
        public static IRabbitMqHost Host(this IRabbitMqBusFactoryConfigurator configurator, string host, ushort port, string virtualHost,
            Action<IRabbitMqHostConfigurator> configure)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));
            if (virtualHost == null)
                throw new ArgumentNullException(nameof(virtualHost));

            var hostConfigurator = new RabbitMqHostConfigurator(host, virtualHost, port);

            configure(hostConfigurator);

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
        public static void ReceiveEndpoint(this IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var queueName = configurator.GetTemporaryQueueName("receiveEndpoint-");

            configurator.ReceiveEndpoint(host, queueName, x =>
            {
                x.AutoDelete = true;
                x.Durable = false;

                configure(x);
            });
        }

        /// <summary>
        /// Registers a management endpoint on the bus, which can be used to control
        /// filters and other management control points on the bus.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The host where the endpoint is to be created</param>
        /// <param name="configure">Configure additional values of the underlying receive endpoint</param>
        /// <returns></returns>
        public static IManagementEndpointConfigurator ManagementEndpoint(this IRabbitMqBusFactoryConfigurator configurator,
            IRabbitMqHost host, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var queueName = configurator.GetTemporaryQueueName("manage-");

            var endpointConfigurator = new RabbitMqReceiveEndpointConfigurator(host, queueName);

            configure?.Invoke(endpointConfigurator);

            configurator.AddBusFactorySpecification(endpointConfigurator);

            var managementEndpointConfigurator = new ManagementEndpointConfigurator(endpointConfigurator);

            return managementEndpointConfigurator;
        }
    }
}