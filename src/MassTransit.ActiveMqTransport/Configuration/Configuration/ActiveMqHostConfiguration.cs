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
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configurators;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class ActiveMqHostConfiguration :
        IActiveMqHostConfiguration
    {
        readonly IActiveMqBusConfiguration _busConfiguration;
        readonly ActiveMqHostSettings _hostSettings;
        readonly IActiveMqHostTopology _hostTopology;
        readonly IActiveMqHostControl _host;

        public ActiveMqHostConfiguration(IActiveMqBusConfiguration busConfiguration, ActiveMqHostSettings hostSettings, IActiveMqHostTopology hostTopology)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = hostSettings;
            _hostTopology = hostTopology;

            _host = new ActiveMqHost(this);

            Description = hostSettings.ToDescription();
        }

        Uri IHostConfiguration.HostAddress => _hostSettings.HostAddress;
        IBusHostControl IHostConfiguration.Host => _host;
        IHostTopology IHostConfiguration.Topology => _hostTopology;

        IActiveMqBusConfiguration IActiveMqHostConfiguration.BusConfiguration => _busConfiguration;
        IActiveMqHostControl IActiveMqHostConfiguration.Host => _host;
        IActiveMqHostTopology IActiveMqHostConfiguration.Topology => _hostTopology;

        public string Description { get; }
        ActiveMqHostSettings IActiveMqHostConfiguration.Settings => _hostSettings;

        public bool Matches(Uri address)
        {
            if (!address.Scheme.Equals("activemq", StringComparison.OrdinalIgnoreCase))
                return false;

            var settings = new ActiveMqHostConfigurator(address).Settings;

            return ActiveMqHostEqualityComparer.Default.Equals(_hostSettings, settings);
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            var settings = _host.Topology.SendTopology.GetSendSettings(address);

            var sessionContextSupervisor = CreateSessionContextSupervisor();

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology());

            return CreateSendTransport(sessionContextSupervisor, configureTopologyFilter, settings.EntityName, DestinationType.Queue);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IActiveMqMessagePublishTopology<T> publishTopology = _hostTopology.Publish<T>();

            var settings = publishTopology.GetSendSettings();

            var sessionContextSupervisor = CreateSessionContextSupervisor();

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, publishTopology.GetBrokerTopology());

            return CreateSendTransport(sessionContextSupervisor, configureTopologyFilter, settings.EntityName, DestinationType.Topic);
        }

        ISessionContextSupervisor CreateSessionContextSupervisor()
        {
            return new ActiveMqSessionContextSupervisor(_host.ConnectionContextSupervisor);
        }

        Task<ISendTransport> CreateSendTransport(ISessionContextSupervisor sessionContextSupervisor, IFilter<SessionContext> configureTopologyFilter,
            string entityName, DestinationType destinationType)
        {
            var transport = new ActiveMqSendTransport(sessionContextSupervisor, configureTopologyFilter, entityName, destinationType);
            transport.Add(sessionContextSupervisor);

            _host.Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            return new ActiveMqReceiveEndpointConfiguration(this, queueName, _busConfiguration.CreateEndpointConfiguration());
        }
    }
}