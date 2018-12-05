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
namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology.Settings;


    public interface IActiveMqBusConfiguration :
        IBusConfiguration
    {
        new IReadOnlyHostCollection<IActiveMqHostConfiguration> Hosts { get; }

        new IActiveMqTopologyConfiguration Topology { get; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IActiveMqEndpointConfiguration CreateEndpointConfiguration();

        /// <summary>
        /// Create a host configuration, by adding a host to the bus
        /// </summary>
        /// <param name="hostSettings"></param>
        /// <returns></returns>
        IActiveMqHostConfiguration CreateHostConfiguration(ActiveMqHostSettings hostSettings);

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="endpointConfiguration"></param>
        /// <returns></returns>
        IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IActiveMqEndpointConfiguration endpointConfiguration);

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="endpointConfiguration"></param>
        /// <returns></returns>
        IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(QueueReceiveSettings settings,
            IActiveMqEndpointConfiguration endpointConfiguration);
    }
}