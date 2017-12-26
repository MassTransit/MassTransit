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
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;
    using Turnout.Configuration;


    public static class ServiceBusTurnoutConfigurationExtensions
    {
        /// <summary>
        /// Configures a Turnout on the receive endpoint, which executes a long-running job and supervises the job until it
        /// completes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busFactoryConfigurator">The bus factory configuration to use a separate endpoint for the control traffic</param>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <param name="host"></param>
        public static void TurnoutEndpoint<T>(this IServiceBusBusFactoryConfigurator busFactoryConfigurator, IServiceBusHost host, string queueName,
            Action<ITurnoutServiceConfigurator<T>> configure)
            where T : class
        {
            string expiredQueueName = $"{queueName}-expired";

            // configure the message expiration endpoint, so it's available at startup
            busFactoryConfigurator.ReceiveEndpoint(host, expiredQueueName, expiredEndpointConfigurator =>
            {
                expiredEndpointConfigurator.SubscribeMessageTopics = false;

                // configure the turnout management endpoint
                var temporaryQueueName = host.GetTemporaryQueueName("turnout-");
                busFactoryConfigurator.ReceiveEndpoint(host, temporaryQueueName, turnoutEndpointConfigurator =>
                {
                    turnoutEndpointConfigurator.PrefetchCount = 100;
                    turnoutEndpointConfigurator.EnableExpress = true;
                    turnoutEndpointConfigurator.SubscribeMessageTopics = false;

                    turnoutEndpointConfigurator.EnableDeadLetteringOnMessageExpiration = true;
                    turnoutEndpointConfigurator.ForwardDeadLetteredMessagesTo = expiredQueueName;

                    turnoutEndpointConfigurator.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);

                    // configure the input queue endpoint
                    busFactoryConfigurator.ReceiveEndpoint(host, queueName, commandEndpointConfigurator =>
                    {
                        commandEndpointConfigurator.SubscribeMessageTopics = false;

                        commandEndpointConfigurator.ConfigureTurnoutEndpoints(busFactoryConfigurator, turnoutEndpointConfigurator, expiredEndpointConfigurator,
                            configure);
                    });
                });
            });
        }
    }
}