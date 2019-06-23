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
namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology.Topologies;


    public class InMemoryBusConfiguration :
        InMemoryEndpointConfiguration,
        IInMemoryBusConfiguration
    {
        readonly IInMemoryTopologyConfiguration _topologyConfiguration;
        readonly IHostCollection<IInMemoryHostConfiguration> _hosts;

        public InMemoryBusConfiguration(IInMemoryTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;

            _hosts = new HostCollection<IInMemoryHostConfiguration>();
        }

        IReadOnlyHostCollection<IInMemoryHostConfiguration> IInMemoryBusConfiguration.Hosts => _hosts;

        public IInMemoryHostConfiguration CreateHostConfiguration(Uri baseAddress, int transportConcurrencyLimit)
        {
            var hostTopology = new InMemoryHostTopology(_topologyConfiguration);

            var hostConfiguration = new InMemoryHostConfiguration(this, baseAddress, transportConcurrencyLimit, hostTopology);

            _hosts.Add(hostConfiguration);

            return hostConfiguration;
        }

        public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IInMemoryEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return _hosts[0].CreateReceiveEndpointConfiguration(queueName, endpointConfiguration);
        }

        IReadOnlyHostCollection IBusConfiguration.Hosts => _hosts;
    }
}
