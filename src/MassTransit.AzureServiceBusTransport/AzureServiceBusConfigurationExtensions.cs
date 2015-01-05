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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Configuration;


    public static class AzureServiceBusConfigurationExtensions
    {
        /// <summary>
        /// Configure and create a bus for Azure Service Bus
        /// </summary>
        /// <param name="selector">Hang off the selector interface for visibility</param>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingAzureServiceBus(this IBusFactory selector, Action<IServiceBusBusFactoryConfigurator> configure)
        {
            return AzureServiceBus.Create(configure);
        }

        /// <summary>
        /// Specify a receive endpoint for the bus, with the specified queue name
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="queueName">The queue name for the receiving endpoint</param>
        /// <param name="configure">The configuration callback</param>
        public static void ReceiveEndpoint(this IInMemoryBusFactoryConfigurator configurator, string queueName,
            Action<IReceiveEndpointConfigurator> configure)
        {
            var endpointConfigurator = new InMemoryReceiveEndpointConfigurator(queueName);

            configure(endpointConfigurator);

            configurator.AddBusFactorySpecification(endpointConfigurator);
        }
    }
}