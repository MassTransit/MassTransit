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
namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology;
    using Topology.Settings;
    using Topology.Topologies;


    public class AmazonSqsBusConfiguration :
        AmazonSqsEndpointConfiguration,
        IAmazonSqsBusConfiguration
    {
        readonly IHostCollection<IAmazonSqsHostConfiguration> _hosts;

        public AmazonSqsBusConfiguration(IAmazonSqsTopologyConfiguration topology)
            : base(topology)
        {
            _hosts = new HostCollection<IAmazonSqsHostConfiguration>();
        }

        public bool DeployTopologyOnly { get; set; }

        public bool TryGetHost(Uri address, out IAmazonSqsHostConfiguration hostConfiguration)
        {
            return _hosts.TryGetHost(address, out hostConfiguration);
        }

        public bool TryGetHost(IAmazonSqsHost host, out IAmazonSqsHostConfiguration hostConfiguration)
        {
            return _hosts.TryGetHost(host, out hostConfiguration);
        }

        public IAmazonSqsHostConfiguration CreateHostConfiguration(AmazonSqsHostSettings settings)
        {
            var hostTopology = CreateHostTopology(settings.HostAddress);

            var hostConfiguration = new AmazonSqsHostConfiguration(this, settings, hostTopology);

            _hosts.Add(hostConfiguration);

            return hostConfiguration;
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            IAmazonSqsEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            var configuration = new AmazonSqsReceiveEndpointConfiguration(_hosts[0], queueName, endpointConfiguration);

            return configuration;
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(QueueReceiveSettings settings,
            IAmazonSqsEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            var configuration = new AmazonSqsReceiveEndpointConfiguration(_hosts[0], settings, endpointConfiguration);

            return configuration;
        }

        public IReadOnlyHostCollection Hosts => _hosts;

        IAmazonSqsHostTopology CreateHostTopology(Uri hostAddress)
        {
            return new AmazonSqsHostTopology(new AmazonSqsMessageNameFormatter(), hostAddress, Topology);
        }
    }
}
