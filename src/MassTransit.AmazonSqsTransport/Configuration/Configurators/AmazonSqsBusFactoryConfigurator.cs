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
namespace MassTransit.AmazonSqsTransport.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;
    using BusConfigurators;
    using Configuration;
    using EndpointSpecifications;
    using GreenPipes;
    using MassTransit.Builders;
    using Topology.Configuration;
    using Topology.Settings;
    using Transport;


    public class AmazonSqsBusFactoryConfigurator :
        BusFactoryConfigurator,
        IAmazonSqsBusFactoryConfigurator,
        IBusFactory
    {
        readonly IAmazonSqsBusConfiguration _configuration;
        readonly IAmazonSqsEndpointConfiguration _busEndpointConfiguration;
        readonly AmazonSqsReceiveSettings _settings;

        public AmazonSqsBusFactoryConfigurator(IAmazonSqsBusConfiguration configuration, IAmazonSqsEndpointConfiguration busEndpointConfiguration)
            : base(configuration, busEndpointConfiguration)
        {
            _configuration = configuration;
            _busEndpointConfiguration = busEndpointConfiguration;

            var queueName = _configuration.Topology.Consume.CreateTemporaryQueueName("bus-");
            _settings = new AmazonSqsReceiveSettings(queueName, false, true);
        }

        public IBusControl CreateBus()
        {
            var busReceiveEndpointConfiguration = _configuration.CreateReceiveEndpointConfiguration(_settings, _busEndpointConfiguration);

            var builder = new ConfigurationBusBuilder(_configuration, busReceiveEndpointConfiguration, BusObservable);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (string.IsNullOrWhiteSpace(_settings.EntityName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");
        }

        public ushort PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public ushort WaitTimeSeconds
        {
            set => _settings.WaitTimeSeconds = value;
        }

        public bool Durable
        {
            set => _settings.Durable = value;
        }

        public bool AutoDelete
        {
            set => _settings.AutoDelete = value;
        }

        public bool Lazy
        {
            set => _settings.Lazy = value;
        }

        public IAmazonSqsHost Host(AmazonSqsHostSettings settings)
        {
            var hostTopology = _configuration.CreateHostTopology(settings.HostAddress);

            var host = new AmazonSqsHost(_configuration, settings, hostTopology);

            _configuration.CreateHostConfiguration(host);

            return host;
        }

        void IAmazonSqsBusFactoryConfigurator.Send<T>(Action<IAmazonSqsMessageSendTopologyConfigurator<T>> configureTopology)
        {
            IAmazonSqsMessageSendTopologyConfigurator<T> configurator = _configuration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        void IAmazonSqsBusFactoryConfigurator.Publish<T>(Action<IAmazonSqsMessagePublishTopologyConfigurator<T>> configureTopology)
        {
            IAmazonSqsMessagePublishTopologyConfigurator<T> configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IAmazonSqsSendTopologyConfigurator SendTopology => _configuration.Topology.Send;
        public new IAmazonSqsPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public bool DeployTopologyOnly
        {
            set => _configuration.DeployTopologyOnly = value;
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            var configuration = _configuration.CreateReceiveEndpointConfiguration(queueName, _configuration.CreateEndpointConfiguration());

            ConfigureReceiveEndpoint(configuration, configureEndpoint);
        }

        public void ReceiveEndpoint(IAmazonSqsHost host, string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configure)
        {
            if (!_configuration.TryGetHost(host, out var hostConfiguration))
                throw new ArgumentException("The host was not configured on this bus", nameof(host));

            var configuration = hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            ConfigureReceiveEndpoint(configuration, configure);
        }

        void ConfigureReceiveEndpoint(IAmazonSqsReceiveEndpointConfiguration configuration, Action<IAmazonSqsReceiveEndpointConfigurator> configure)
        {
            configuration.ConnectConsumerConfigurationObserver(this);
            configuration.ConnectSagaConfigurationObserver(this);
            configuration.ConnectHandlerConfigurationObserver(this);

            configure?.Invoke(configuration.Configurator);

            var specification = new ConfigurationReceiveEndpointSpecification(configuration);

            AddReceiveEndpointSpecification(specification);
        }
    }
}