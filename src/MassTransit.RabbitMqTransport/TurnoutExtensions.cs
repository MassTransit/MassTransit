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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using Turnout.Configuration;


    public static class TurnoutExtensions
    {
        /// <summary>
        /// Configures a Turnout on the receive endpoint, which executes a long-running job and supervises the job until it
        /// completes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busFactoryConfigurator">The bus factory configuration to use a separate endpoint for the control traffic</param>
        /// <param name="queueName">The receive queue name for commands</param>
        /// <param name="configure"></param>
        /// <param name="host">The host on which to configure the endpoint</param>
        public static void TurnoutEndpoint<T>(this IRabbitMqBusFactoryConfigurator busFactoryConfigurator, IRabbitMqHost host, string queueName,
            Action<ITurnoutHostConfigurator<T>> configure)
            where T : class
        {
            string deadLetterQueueName = $"{queueName}-expired";

            // configure the dead letter endpoint, so it's available at startup
            busFactoryConfigurator.ReceiveEndpoint(host, deadLetterQueueName, deadLetterConfigurator =>
            {
                // configure the turnout management endpoint
                var temporaryQueueName = host.GetTemporaryQueueName("turnout-");
                busFactoryConfigurator.ReceiveEndpoint(host, temporaryQueueName, turnoutConfigurator =>
                {
                    turnoutConfigurator.PrefetchCount = 100;
                    turnoutConfigurator.AutoDelete = true;
                    turnoutConfigurator.Durable = false;

                    turnoutConfigurator.DeadLetterExchange = deadLetterQueueName;

                    // configure the input queue endpoint
                    busFactoryConfigurator.ReceiveEndpoint(host, queueName, configurator =>
                    {
                        configurator.ConfigureTurnoutEndpoints(busFactoryConfigurator, turnoutConfigurator, deadLetterConfigurator, configure);
                    });
                });
            });
        }
    }
}