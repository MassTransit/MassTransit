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


    public static class RabbitMqHostConfigurationExtensions
    {
        /// <summary>
        ///     Configure a RabbitMQ host using the configuration API
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The URI host address of the RabbitMQ host (rabbitmq://host:port/vhost)</param>
        /// <param name="configure"></param>
        public static void Host(this RabbitMqTransportConfigurator configurator, Uri hostAddress,
            Action<RabbitMqHostConfigurator> configure)
        {
            var hostConfigurator = new RabbitMqHostConfiguratorImpl(hostAddress);
            configure(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        public static void Endpoint(this RabbitMqTransportConfigurator configurator, string queueName,
            Action<RabbitMqEndpointConfigurator> configure)
        {
            var endpointConfigurator = new RabbitMqEndpointConfiguratorImpl(queueName);

            configure(endpointConfigurator);

            configurator.Endpoint(endpointConfigurator.Settings);
        }
    }
}