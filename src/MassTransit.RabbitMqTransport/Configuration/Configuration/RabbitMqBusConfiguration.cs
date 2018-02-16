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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology;
    using Topology.Settings;
    using Topology.Topologies;
    using Transport;


    public class RabbitMqBusConfiguration :
        RabbitMqEndpointConfiguration,
        IRabbitMqBusConfiguration
    {
        readonly IHostCollection<IRabbitMqHostConfiguration> _hosts;

        public RabbitMqBusConfiguration(IRabbitMqTopologyConfiguration topology)
            : base(topology)
        {
            _hosts = new HostCollection<IRabbitMqHostConfiguration>();
        }

        public bool DeployTopologyOnly { get; set; }

        public bool TryGetHost(Uri address, out IRabbitMqHostConfiguration hostConfiguration)
        {
            return _hosts.TryGetHost(address, out hostConfiguration);
        }

        public bool TryGetHost(IRabbitMqHost host, out IRabbitMqHostConfiguration hostConfiguration)
        {
            return _hosts.TryGetHost(host, out hostConfiguration);
        }

        public IRabbitMqHostTopology CreateHostTopology(Uri hostAddress)
        {
            return new RabbitMqHostTopology(new FanoutExchangeTypeSelector(), new RabbitMqMessageNameFormatter(), hostAddress, Topology);
        }

        public IRabbitMqHostConfiguration CreateHostConfiguration(IRabbitMqHostControl host)
        {
            var hostConfiguration = new RabbitMqHostConfiguration(this, host);

            _hosts.Add(hostConfiguration);

            return hostConfiguration;
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IRabbitMqEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            var configuration = new RabbitMqReceiveEndpointConfiguration(_hosts[0], queueName, endpointConfiguration);

            return configuration;
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(RabbitMqReceiveSettings settings, IRabbitMqEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            var configuration = new RabbitMqReceiveEndpointConfiguration(_hosts[0], settings, endpointConfiguration);

            return configuration;
        }

        public IReadOnlyHostCollection Hosts => _hosts;
    }
}