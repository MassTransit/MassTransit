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
    using System;
    using MassTransit.Configuration;
    using Settings;
    using Topology;


    public interface IServiceBusBusConfiguration :
        IBusConfiguration,
        IServiceBusEndpointConfiguration
    {
        new IServiceBusTopologyConfiguration Topology { get; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        /// <summary>
        /// Returns all the hosts
        /// </summary>
        new IServiceBusHostConfiguration[] Hosts { get; }

        /// <summary>
        /// Return the host associated with the specified address, if present.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        bool TryGetHost(Uri address, out IServiceBusHostConfiguration host);

        /// <summary>
        /// Return the host if it is part of the configuration
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        /// <param name="hostConfiguration"></param>
        bool TryGetHost(IServiceBusHost host, out IServiceBusHostConfiguration hostConfiguration);

        ServiceBusHost GetHost(Uri address);

        /// <summary>
        /// Create a host configuration, by adding a host to the bus
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        IServiceBusHostConfiguration CreateHostConfiguration(ServiceBusHost host);

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="endpointConfiguration"></param>
        /// <returns></returns>
        IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IServiceBusEndpointConfiguration endpointConfiguration);

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="endpointConfiguration"></param>
        /// <returns></returns>
        IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration);

        IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(Uri hostAddress, Uri inputAddress);

        /// <summary>
        /// Create a subscription endpoint configuration for the default host
        /// </summary>
        /// <param name="subscriptionName"></param>
        /// <param name="endpointConfiguration"></param>
        /// <param name="topicPath"></param>
        /// <returns></returns>
        IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(string topicPath, string subscriptionName,
            IServiceBusEndpointConfiguration endpointConfiguration);

        IServiceBusHostTopology CreateHostTopology();
    }
}