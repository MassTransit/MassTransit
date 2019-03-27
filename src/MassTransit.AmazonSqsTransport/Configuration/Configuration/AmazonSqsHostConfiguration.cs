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
    using System.Threading.Tasks;
    using Configurators;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class AmazonSqsHostConfiguration :
        IAmazonSqsHostConfiguration
    {
        readonly IAmazonSqsBusConfiguration _busConfiguration;
        readonly IAmazonSqsHostControl _host;
        readonly IAmazonSqsHostTopology _topology;

        public AmazonSqsHostConfiguration(IAmazonSqsBusConfiguration busConfiguration, AmazonSqsHostSettings settings, IAmazonSqsHostTopology topology)
        {
            Settings = settings;
            _topology = topology;
            _busConfiguration = busConfiguration;

            _host = new AmazonSqsHost(this);
        }

        IBusHostControl IHostConfiguration.Host => _host;
        Uri IHostConfiguration.HostAddress => Settings.HostAddress;
        IHostTopology IHostConfiguration.Topology => _topology;

        public AmazonSqsHostSettings Settings { get; }

        IAmazonSqsBusConfiguration IAmazonSqsHostConfiguration.BusConfiguration => _busConfiguration;
        IAmazonSqsHostControl IAmazonSqsHostConfiguration.Host => _host;
        IAmazonSqsHostTopology IAmazonSqsHostConfiguration.Topology => _topology;

        public bool Matches(Uri address)
        {
            if (!address.Scheme.Equals("amazonsqs", StringComparison.OrdinalIgnoreCase))
                return false;

            var settings = new AmazonSqsHostConfigurator(address).Settings;

            return AmazonSqsHostEqualityComparer.Default.Equals(Settings, settings);
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            var settings = _topology.SendTopology.GetSendSettings(address);

            var clientContextSupervisor = _busConfiguration.ClientContextSupervisorFactory.Create(_host.ConnectionContextSupervisor);

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology());

            var transport = new QueueSendTransport(clientContextSupervisor, configureTopologyFilter, settings.EntityName);
            transport.Add(clientContextSupervisor);

            _host.Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IAmazonSqsMessagePublishTopology<T> publishTopology = _topology.Publish<T>();

            var sendSettings = publishTopology.GetPublishSettings();

            var clientContextSupervisor = _busConfiguration.ClientContextSupervisorFactory.Create(_host.ConnectionContextSupervisor);

            var configureTopologyFilter = new ConfigureTopologyFilter<PublishSettings>(sendSettings, publishTopology.GetBrokerTopology());

            var sendTransport = new TopicSendTransport(clientContextSupervisor, configureTopologyFilter, sendSettings.EntityName);
            sendTransport.Add(clientContextSupervisor);

            _host.Add(sendTransport);

            return Task.FromResult<ISendTransport>(sendTransport);
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            return new AmazonSqsReceiveEndpointConfiguration(this, queueName, _busConfiguration.CreateEndpointConfiguration());
        }
    }
}
