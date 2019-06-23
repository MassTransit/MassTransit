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
    using Builders;
    using MassTransit.Configuration;


    public class InMemoryReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IInMemoryReceiveEndpointConfiguration,
        IInMemoryReceiveEndpointConfigurator
    {
        readonly IInMemoryEndpointConfiguration _endpointConfiguration;
        readonly IInMemoryHostConfiguration _hostConfiguration;
        readonly string _queueName;

        public InMemoryReceiveEndpointConfiguration(IInMemoryHostConfiguration hostConfiguration, string queueName,
            IInMemoryEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _queueName = queueName;
            _endpointConfiguration = endpointConfiguration;

            HostAddress = hostConfiguration.Host.Address;
            InputAddress = new Uri(hostConfiguration.Host.Address, $"{queueName}");
        }

        IInMemoryReceiveEndpointConfigurator IInMemoryReceiveEndpointConfiguration.Configurator => this;

        public int ConcurrencyLimit { get; set; }

        IInMemoryTopologyConfiguration IInMemoryEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public override IReceiveEndpoint Build()
        {
            var builder = new InMemoryReceiveEndpointBuilder(_hostConfiguration.Host, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            var transport = _hostConfiguration.Host.GetReceiveTransport(_queueName, receiveEndpointContext);

            return CreateReceiveEndpoint(_queueName, transport, receiveEndpointContext);
        }
    }
}
