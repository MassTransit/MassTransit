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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;
    using Settings;


    public interface IServiceBusHostConfiguration :
        IHostConfiguration
    {
        IServiceBusBusConfiguration BusConfiguration { get; }

        new ServiceBusHost Host { get; }

        /// <summary>
        /// Create a receive endpoint configuration using the specified host
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <returns></returns>
        IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName);

        /// <summary>
        /// Create a subscription endpoint on the host
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(SubscriptionEndpointSettings settings);
    }
}