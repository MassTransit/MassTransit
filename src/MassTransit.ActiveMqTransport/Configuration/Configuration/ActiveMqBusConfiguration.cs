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
    using System;
    using MassTransit.Configuration;
    using Topology;
    using Topology.Settings;
    using Topology.Topologies;


    public class ActiveMqBusConfiguration :
        ActiveMqEndpointConfiguration,
        IActiveMqBusConfiguration
    {
        readonly IHostCollection<IActiveMqHostConfiguration> _hosts;

        public ActiveMqBusConfiguration(IActiveMqTopologyConfiguration topology)
            : base(topology)
        {
            _hosts = new HostCollection<IActiveMqHostConfiguration>();
        }

        public bool DeployTopologyOnly { get; set; }

        public bool TryGetHost(Uri address, out IActiveMqHostConfiguration hostConfiguration)
        {
            return _hosts.TryGetHost(address, out hostConfiguration);
        }

        public bool TryGetHost(IActiveMqHost host, out IActiveMqHostConfiguration hostConfiguration)
        {
            return _hosts.TryGetHost(host, out hostConfiguration);
        }

        public IActiveMqHostTopology CreateHostTopology(Uri hostAddress)
        {
            return new ActiveMqHostTopology(new ActiveMqMessageNameFormatter(), hostAddress, Topology);
        }

        public IActiveMqHostConfiguration CreateHostConfiguration(IActiveMqHostControl host)
        {
            var hostConfiguration = new ActiveMqHostConfiguration(this, host);

            _hosts.Add(hostConfiguration);

            return hostConfiguration;
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IActiveMqEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            var configuration = new ActiveMqReceiveEndpointConfiguration(_hosts[0], queueName, endpointConfiguration);

            return configuration;
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(QueueReceiveSettings settings, IActiveMqEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            var configuration = new ActiveMqReceiveEndpointConfiguration(_hosts[0], settings, endpointConfiguration);

            return configuration;
        }

        public IReadOnlyHostCollection Hosts => _hosts;
    }
}