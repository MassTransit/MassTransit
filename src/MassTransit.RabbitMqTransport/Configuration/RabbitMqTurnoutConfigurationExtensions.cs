// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Turnout.Configuration;


    public static class RabbitMqTurnoutConfigurationExtensions
    {
        /// <summary>
        /// Creates a Turnout endpoint on the bus, which is capable of executing long-running jobs without hanging the consumer pipeline.
        /// Multiple receive endpoints are created, including the main queue, an expired queue, and a management queue for communicating
        /// back to the turnout coordinator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busFactoryConfigurator">The bus factory configuration to use a separate endpoint for the control traffic</param>
        /// <param name="queueName">The receive queue name for commands</param>
        /// <param name="configure"></param>
        /// <param name="host">The host on which to configure the endpoint</param>
        public static void TurnoutEndpoint<T>(this IRabbitMqBusFactoryConfigurator busFactoryConfigurator, IRabbitMqHost host, string queueName,
            Action<ITurnoutServiceConfigurator<T>> configure)
            where T : class
        {
            string expiredQueueName = $"{queueName}-expired";

            // configure the message expiration endpoint, so it's available at startup
            busFactoryConfigurator.ReceiveEndpoint(host, expiredQueueName, expiredEndpointConfigurator =>
            {
                // configure the turnout management endpoint
                var temporaryQueueName = busFactoryConfigurator.CreateTemporaryQueueName("turnout-");
                busFactoryConfigurator.ReceiveEndpoint(host, temporaryQueueName, turnoutEndpointConfigurator =>
                {
                    turnoutEndpointConfigurator.PrefetchCount = 100;
                    turnoutEndpointConfigurator.AutoDelete = true;
                    turnoutEndpointConfigurator.Durable = false;

                    turnoutEndpointConfigurator.DeadLetterExchange = expiredQueueName;

                    // configure the input queue endpoint
                    busFactoryConfigurator.ReceiveEndpoint(host, queueName, commandEndpointConfigurator =>
                    {
                        commandEndpointConfigurator.ConfigureTurnoutEndpoints(busFactoryConfigurator, turnoutEndpointConfigurator, expiredEndpointConfigurator,
                            configure);
                    });
                });
            });
        }
    }
}