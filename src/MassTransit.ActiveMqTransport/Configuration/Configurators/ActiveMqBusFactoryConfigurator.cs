// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using BusConfigurators;
    using EndpointSpecifications;
    using GreenPipes;
    using MassTransit.Builders;
    using Topology;
    using Topology.Settings;
    using Topology.Topologies;
    using Transport;
    using Transports;


    public class ActiveMqBusFactoryConfigurator :
        BusFactoryConfigurator<IBusBuilder>,
        IActiveMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly IActiveMqEndpointConfiguration _configuration;
        readonly BusHostCollection<ActiveMqHost> _hosts;
        readonly QueueReceiveSettings _settings;

        public ActiveMqBusFactoryConfigurator(IActiveMqEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;

            _hosts = new BusHostCollection<ActiveMqHost>();

            var queueName = _configuration.Topology.Consume.CreateTemporaryQueueName("bus-");
            _settings = new QueueReceiveSettings(queueName, false, true);
        }

        public IBusControl CreateBus()
        {
            var builder = new ActiveMqBusBuilder(_hosts, _settings, _configuration);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_hosts.Count == 0)
                yield return this.Failure("Host", "At least one host must be defined");

            if (string.IsNullOrWhiteSpace(_settings.EntityName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");
        }

        public ushort PrefetchCount
        {
            set { _settings.PrefetchCount = value; }
        }

        public bool Durable
        {
            set { _settings.Durable = value; }
        }

        public bool AutoDelete
        {
            set { _settings.AutoDelete = value; }
        }

        public bool PurgeOnStartup
        {
            set { _settings.PurgeOnStartup = value; }
        }

        public bool Lazy
        {
            set { _settings.Lazy = value; }
        }

        public IActiveMqHost Host(ActiveMqHostSettings settings)
        {
            var hostTopology = new ActiveMqHostTopology(new ActiveMqMessageNameFormatter(), settings.HostAddress, _configuration.Topology);

            var host = new ActiveMqHost(settings, hostTopology);
            _hosts.Add(host);

            return host;
        }

        public string CreateTemporaryQueueName(string prefix)
        {
            return _configuration.Topology.Consume.CreateTemporaryQueueName(prefix);
        }

        void IActiveMqBusFactoryConfigurator.Send<T>(Action<IActiveMqMessageSendTopologyConfigurator<T>> configureTopology)
        {
            IActiveMqMessageSendTopologyConfigurator<T> configurator = _configuration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        void IActiveMqBusFactoryConfigurator.Publish<T>(Action<IActiveMqMessagePublishTopologyConfigurator<T>> configureTopology)
        {
            IActiveMqMessagePublishTopologyConfigurator<T> configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IActiveMqSendTopologyConfigurator SendTopology => _configuration.Topology.Send;
        public new IActiveMqPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_hosts.Count == 0)
                throw new ArgumentException("At least one host must be configured before configuring a receive endpoint");

            ReceiveEndpoint(_hosts[0], queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(IActiveMqHost host, string queueName, Action<IActiveMqReceiveEndpointConfigurator> configure)
        {
            var rabbitMqHost = host as ActiveMqHost ?? throw new ArgumentException("The host must be an ActiveMQ host", nameof(host));

            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var specification = new ActiveMqReceiveEndpointSpecification(rabbitMqHost, endpointTopologySpecification, queueName);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }
    }
}