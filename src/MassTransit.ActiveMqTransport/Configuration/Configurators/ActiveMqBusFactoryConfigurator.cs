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
    using BusConfigurators;
    using Configuration;
    using Definition;
    using GreenPipes;
    using MassTransit.Builders;
    using Topology;
    using Topology.Settings;


    public class ActiveMqBusFactoryConfigurator :
        BusFactoryConfigurator,
        IActiveMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly IActiveMqBusConfiguration _configuration;
        readonly IActiveMqEndpointConfiguration _busEndpointConfiguration;
        readonly QueueReceiveSettings _settings;

        public ActiveMqBusFactoryConfigurator(IActiveMqBusConfiguration configuration, IActiveMqEndpointConfiguration busEndpointConfiguration)
            : base(configuration, busEndpointConfiguration)
        {
            _configuration = configuration;
            _busEndpointConfiguration = busEndpointConfiguration;

            var busQueueName = _configuration.Topology.Consume.CreateTemporaryQueueName("bus");
            _settings = new QueueReceiveSettings(busQueueName, false, true);
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

        public IActiveMqHost Host(ActiveMqHostSettings settings)
        {
            var hostConfiguration = _configuration.CreateHostConfiguration(settings);

            return hostConfiguration.Host;
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

        public bool DeployTopologyOnly
        {
            set => _configuration.DeployTopologyOnly = value;
        }

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            var configuration = _configuration.CreateReceiveEndpointConfiguration(queueName, _configuration.CreateEndpointConfiguration());

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, x => x.Apply(definition, configureEndpoint));
        }

        public override void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint)
        {
            var configuration = _configuration.CreateReceiveEndpointConfiguration(queueName, _configuration.CreateEndpointConfiguration());

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, configureEndpoint);
        }

        public void ReceiveEndpoint(IActiveMqHost host, IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            var configuration = CreateConfiguration(host, queueName);

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(IActiveMqHost host, string queueName, Action<IActiveMqReceiveEndpointConfigurator> configure)
        {
            var configuration = CreateConfiguration(host, queueName);

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, configure);
        }

        IActiveMqReceiveEndpointConfiguration CreateConfiguration(IActiveMqHost host, string queueName)
        {
            if (!_configuration.Hosts.TryGetHost(host, out var hostConfiguration))
                throw new ArgumentException("The host was not configured on this bus", nameof(host));

            return hostConfiguration.CreateReceiveEndpointConfiguration(queueName);
        }
    }
}
