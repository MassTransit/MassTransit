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
namespace MassTransit.RabbitMqTransport.Configurators
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


    public class RabbitMqBusFactoryConfigurator :
        BusFactoryConfigurator,
        IRabbitMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly IRabbitMqEndpointConfiguration _busEndpointConfiguration;
        readonly IRabbitMqBusConfiguration _configuration;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqBusFactoryConfigurator(IRabbitMqBusConfiguration configuration, IRabbitMqEndpointConfiguration busEndpointConfiguration)
            : base(configuration, busEndpointConfiguration)
        {
            _configuration = configuration;
            _busEndpointConfiguration = busEndpointConfiguration;

            var busQueueName = _configuration.Topology.Consume.CreateTemporaryQueueName("bus");

            _settings = new RabbitMqReceiveSettings(busQueueName, busEndpointConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, false,
                true);

            _settings.AutoDeleteAfter(TimeSpan.FromMinutes(1));
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

            if (string.IsNullOrWhiteSpace(_settings.QueueName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");
        }

        public ushort PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public bool Durable
        {
            set => _settings.Durable = value;
        }

        public bool Exclusive
        {
            set => _settings.Exclusive = value;
        }

        public bool AutoDelete
        {
            set => _settings.AutoDelete = value;
        }

        public string ExchangeType
        {
            set => _settings.ExchangeType = value;
        }

        public bool PurgeOnStartup
        {
            set => _settings.PurgeOnStartup = value;
        }

        public int ConsumerPriority
        {
            set => _settings.ConsumerPriority = value;
        }

        public bool ExclusiveConsumer
        {
            set => _settings.ExclusiveConsumer = value;
        }

        public bool Lazy
        {
            set => _settings.Lazy = value;
        }

        public TimeSpan? QueueExpiration
        {
            set => _settings.QueueExpiration = value;
        }

        public void SetQueueArgument(string key, object value)
        {
            _settings.SetQueueArgument(key, value);
        }

        public void SetQueueArgument(string key, TimeSpan value)
        {
            _settings.SetQueueArgument(key, value);
        }

        public void SetExchangeArgument(string key, object value)
        {
            _settings.SetExchangeArgument(key, value);
        }

        public void SetExchangeArgument(string key, TimeSpan value)
        {
            _settings.SetExchangeArgument(key, value);
        }

        public void EnablePriority(byte maxPriority)
        {
            _settings.EnablePriority(maxPriority);
        }

        public IRabbitMqHost Host(RabbitMqHostSettings settings)
        {
            var hostConfiguration = _configuration.CreateHostConfiguration(settings);

            return hostConfiguration.Host;
        }

        void IRabbitMqBusFactoryConfigurator.Send<T>(Action<IRabbitMqMessageSendTopologyConfigurator<T>> configureTopology)
        {
            IRabbitMqMessageSendTopologyConfigurator<T> configurator = _configuration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        void IRabbitMqBusFactoryConfigurator.Publish<T>(Action<IRabbitMqMessagePublishTopologyConfigurator<T>> configureTopology)
        {
            IRabbitMqMessagePublishTopologyConfigurator<T> configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IRabbitMqSendTopologyConfigurator SendTopology => _configuration.Topology.Send;
        public new IRabbitMqPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public bool DeployTopologyOnly
        {
            set => _configuration.DeployTopologyOnly = value;
        }

        public void OverrideDefaultBusEndpointQueueName(string queueName)
        {
            _settings.ExchangeName = queueName;
            _settings.QueueName = queueName;
        }

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            var configuration = CreateConfiguration(queueName);

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, x => x.Apply(definition, configureEndpoint));
        }

        public override void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            var configuration = CreateConfiguration(queueName);

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, configureEndpoint);
        }

        public void ReceiveEndpoint(IRabbitMqHost host, IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            var configuration = CreateConfiguration(host, queueName);

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(IRabbitMqHost host, string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var configuration = CreateConfiguration(host, queueName);

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, configure);
        }

        IRabbitMqReceiveEndpointConfiguration CreateConfiguration(string queueName)
        {
            var settings = new RabbitMqReceiveSettings(queueName, _configuration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType,
                true, false);

            return _configuration.CreateReceiveEndpointConfiguration(settings, _configuration.CreateEndpointConfiguration());
        }

        IRabbitMqReceiveEndpointConfiguration CreateConfiguration(IRabbitMqHost host, string queueName)
        {
            if (!_configuration.Hosts.TryGetHost(host, out var hostConfiguration))
                throw new ArgumentException("The host was not configured on this bus", nameof(host));

            return hostConfiguration.CreateReceiveEndpointConfiguration(queueName);
        }

    }
}
