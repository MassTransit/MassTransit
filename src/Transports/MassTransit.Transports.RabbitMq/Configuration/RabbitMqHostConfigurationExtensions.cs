// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Transports.RabbitMq.Configuration;
    using Transports.RabbitMq.Configuration.Configurators;


    public static class RabbitMqHostConfigurationExtensions
    {
        /// <summary>
        ///     Configure a RabbitMQ host using the configuration API
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the RabbitMQ host (rabbitmq://host:port/vhost)</param>
        /// <param name="configure"></param>
        public static RabbitMqHostSettings Host(this IRabbitMqServiceBusFactoryConfigurator configurator, Uri hostAddress,
            Action<IRabbitMqHostConfigurator> configure)
        {
            var hostConfigurator = new RabbitMqHostConfigurator(hostAddress);

            configure(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);

            return hostConfigurator.Settings;
        }

        /// <summary>
        /// Declare a ReceiveEndpoint on the broker and configure the endpoint settings and message consumers.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostSettings">The host for this endpoint</param>
        /// <param name="queueName">The input queue name</param>
        /// <param name="configure">The configuration method</param>
        public static void ReceiveEndpoint(this IRabbitMqServiceBusFactoryConfigurator configurator, RabbitMqHostSettings hostSettings,
            string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var endpointConfigurator = new RabbitMqReceiveEndpointConfigurator(hostSettings, queueName);

            configure(endpointConfigurator);

            configurator.AddServiceBusFactoryBuilderConfigurator(endpointConfigurator);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a broker-assigned queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostSettings"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IRabbitMqServiceBusFactoryConfigurator configurator, RabbitMqHostSettings hostSettings,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var endpointConfigurator = new RabbitMqReceiveEndpointConfigurator(hostSettings);

            endpointConfigurator.AutoDelete();
            endpointConfigurator.Durable(false);
            endpointConfigurator.Exclusive();

            configure(endpointConfigurator);

            configurator.AddServiceBusFactoryBuilderConfigurator(endpointConfigurator);
        }
    }
}