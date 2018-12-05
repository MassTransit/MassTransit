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
    using Topology;
    using Topology.Topologies;


    public class ServiceBusBusConfiguration :
        ServiceBusEndpointConfiguration,
        IServiceBusBusConfiguration
    {
        readonly IHostCollection<IServiceBusHostConfiguration> _hosts;

        public ServiceBusBusConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _hosts = new HostCollection<IServiceBusHostConfiguration>();
        }

        IReadOnlyHostCollection IBusConfiguration.Hosts => _hosts;
        public bool DeployTopologyOnly { get; set; }

        IReadOnlyHostCollection<IServiceBusHostConfiguration> IServiceBusBusConfiguration.Hosts => _hosts;

        public IServiceBusHostConfiguration CreateHostConfiguration(ServiceBusHostSettings settings)
        {
            var hostTopology = CreateHostTopology();

            var hostConfiguration = new ServiceBusHostConfiguration(this, settings, hostTopology);

            _hosts.Add(hostConfiguration);

            return hostConfiguration;
        }

        public IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return _hosts[0].CreateReceiveEndpointConfiguration(queueName);
        }

        public IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return new ServiceBusReceiveEndpointConfiguration(_hosts[0], endpointConfiguration, settings);
        }

        public IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(string topicPath, string subscriptionName)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            var settings = new SubscriptionEndpointSettings(topicPath, subscriptionName);

            return _hosts[0].CreateSubscriptionEndpointConfiguration(settings);
        }

        IServiceBusHostTopology CreateHostTopology()
        {
            return new ServiceBusHostTopology(Topology);
        }
    }
}