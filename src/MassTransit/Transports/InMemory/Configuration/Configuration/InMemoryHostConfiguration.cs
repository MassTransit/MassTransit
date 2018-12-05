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
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using MassTransit.Topology;


    public class InMemoryHostConfiguration :
        IInMemoryHostConfiguration
    {
        readonly IInMemoryBusConfiguration _busConfiguration;
        readonly InMemoryHost _host;

        public InMemoryHostConfiguration(IInMemoryBusConfiguration busConfiguration, Uri baseAddress, int transportConcurrencyLimit, IHostTopology hostTopology)
        {
            _busConfiguration = busConfiguration;

            _host = new InMemoryHost(this, transportConcurrencyLimit, hostTopology, baseAddress);
        }

        Uri IHostConfiguration.HostAddress => _host.Address;
        IBusHostControl IHostConfiguration.Host => _host;
        public IHostTopology Topology => _host.Topology;

        public bool Matches(Uri address)
        {
            return address.ToString().StartsWith(_host.Address.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            return _host.GetSendTransport(address);
        }

        public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            return new InMemoryReceiveEndpointConfiguration(this, queueName, _busConfiguration.CreateEndpointConfiguration());
        }

        public IInMemoryHostControl Host => _host;
    }
}