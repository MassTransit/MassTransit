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
    using System.Linq;
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

        public bool DeployTopologyOnly { get; set; }

        public IServiceBusHostConfiguration[] Hosts => _hosts.Hosts;

        public bool TryGetHost(Uri address, out IServiceBusHostConfiguration host)
        {
            return _hosts.TryGetHost(address, out host);
        }

        public bool TryGetHost(IServiceBusHost host, out IServiceBusHostConfiguration hostConfiguration)
        {
            return _hosts.TryGetHost(host, out hostConfiguration);
        }

        public ServiceBusHost GetHost(Uri address)
        {
            var host = Hosts.Where(x => x.Host.Matches(address))
                .OrderByDescending(x => address.AbsolutePath.StartsWith(x.Host.Address.AbsolutePath, StringComparison.OrdinalIgnoreCase)
                    ? 1
                    : 0)
                .FirstOrDefault();

            if (host == null)
                throw new EndpointNotFoundException($"The host was not found for the specified address: {address}");

            return host.Host;
        }

        public IServiceBusHostConfiguration CreateHostConfiguration(ServiceBusHost host)
        {
            var hostConfiguration = new ServiceBusHostConfiguration(this, host);

            _hosts.Add(hostConfiguration);

            return hostConfiguration;
        }

        public IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(Uri hostAddress, Uri inputAddress)
        {
            return new BrokeredMessageReceiverServiceBusEndpointConfiguration(CreateEndpointConfiguration(), hostAddress, inputAddress);
        }

        public IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            IServiceBusEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return new ServiceBusReceiveEndpointConfiguration(_hosts[0], endpointConfiguration, queueName);
        }

        public IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return new ServiceBusReceiveEndpointConfiguration(_hosts[0], endpointConfiguration, settings);
        }

        public IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(string topicPath, string subscriptionName,
            IServiceBusEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return new ServiceBusSubscriptionEndpointConfiguration(_hosts[0], endpointConfiguration, topicPath, subscriptionName);
        }

        public IServiceBusHostTopology CreateHostTopology()
        {
            return new ServiceBusHostTopology(Topology);
        }

        IReadOnlyHostCollection IBusConfiguration.Hosts => _hosts;
    }
}