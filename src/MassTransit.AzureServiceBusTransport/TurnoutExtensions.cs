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
    using AzureServiceBusTransport;
    using Turnout.Configuration;


    public static class TurnoutExtensions
    {
        /// <summary>
        /// Configures a Turnout on the receive endpoint, which executes a long-running job and supervises the job until it
        /// completes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator">The receive endpoint configurator</param>
        /// <param name="busFactoryConfigurator">The bus factory configuration to use a separate endpoint for the control traffic</param>
        /// <param name="configure"></param>
        public static void Turnout<T>(this IServiceBusReceiveEndpointConfigurator configurator, IServiceBusBusFactoryConfigurator busFactoryConfigurator,
            Action<ITurnoutHostConfigurator<T>> configure)
            where T : class
        {
            var temporaryQueueName = configurator.Host.GetTemporaryQueueName("turnout-");

            busFactoryConfigurator.ReceiveEndpoint(configurator.Host, temporaryQueueName, turnoutEndpointConfigurator =>
            {
                turnoutEndpointConfigurator.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                turnoutEndpointConfigurator.EnableExpress = true;

                configurator.ConfigureTurnoutEndpoints(busFactoryConfigurator, turnoutEndpointConfigurator, configure);
            });
        }
    }
}