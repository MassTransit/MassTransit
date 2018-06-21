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


    public class InMemoryBusConfiguration :
        InMemoryEndpointConfiguration,
        IInMemoryBusConfiguration
    {
        readonly IHostCollection<IInMemoryHostConfiguration> _hosts;

        public InMemoryBusConfiguration(IInMemoryTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _hosts = new HostCollection<IInMemoryHostConfiguration>();
        }

        public IInMemoryHostConfiguration[] Hosts => _hosts.Hosts;

        public bool TryGetHost(Uri address, out IInMemoryHostConfiguration host)
        {
            return _hosts.TryGetHost(address, out host);
        }

        public IInMemoryHostConfiguration CreateHostConfiguration(InMemoryHost host)
        {
            var hostConfiguration = new InMemoryHostConfiguration(this, host);

            _hosts.Add(hostConfiguration);

            return hostConfiguration;
        }

        public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IInMemoryEndpointConfiguration endpointConfiguration)
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return new InMemoryReceiveEndpointConfiguration(_hosts[0], queueName, endpointConfiguration);
        }

        IReadOnlyHostCollection IBusConfiguration.Hosts => _hosts;
    }
}