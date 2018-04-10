// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public static class ServiceBusAddressExtensions
    {
        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus intance
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host">The service bus host</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IManagementEndpointConfigurator ManagementEndpoint(this IServiceBusBusFactoryConfigurator configurator,
            IServiceBusHost host, Action<IReceiveEndpointConfigurator> configure = null)
        {
            var queueName = host.Topology.CreateTemporaryQueueName("manage-");

            IServiceBusReceiveEndpointConfigurator specification = null;
            configurator.ReceiveEndpoint(host, queueName, x =>
            {
                x.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
                x.EnableExpress = true;

                configure?.Invoke(x);

                specification = x;
            });

            return new ManagementEndpointConfigurator(specification);
        }
    }
}