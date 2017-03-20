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
namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using BusConfigurators;
    using GreenPipes;
    using MassTransit.Builders;
    using Specifications;
    using Topology;
    using Transport;
    using Transports;


    public class RabbitMqBusFactoryConfigurator :
        BusFactoryConfigurator<IBusBuilder>,
        IRabbitMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly BusHostCollection<RabbitMqHost> _hosts;
        readonly RabbitMqReceiveSettings _settings;
        readonly IRabbitMqEndpointConfiguration _configuration;

        public RabbitMqBusFactoryConfigurator(IRabbitMqEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;

            _hosts = new BusHostCollection<RabbitMqHost>();

            _settings = new RabbitMqReceiveSettings("ignore", "fanout", false, true);
            _settings.SetQueueArgument("x-expires", TimeSpan.FromMinutes(1));
            _settings.SetExchangeArgument("x-expires", TimeSpan.FromMinutes(1));
        }

        public IBusControl CreateBus()
        {
            var builder = new RabbitMqBusBuilder(_hosts, _settings, _configuration);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_hosts.Count == 0)
                yield return this.Failure("Host", "At least one host must be defined");
            if (string.IsNullOrWhiteSpace(_settings.QueueName))
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

        public string QueueName
        {
            set { _settings.QueueName = value; }
        }

        public bool Exclusive
        {
            set { _settings.Exclusive = value; }
        }

        public bool AutoDelete
        {
            set { _settings.AutoDelete = value; }
        }

        public string ExchangeType
        {
            set { _settings.ExchangeType = value; }
        }

        public bool PurgeOnStartup
        {
            set { _settings.PurgeOnStartup = value; }
        }

        public bool Lazy
        {
            set { _settings.Lazy = value; }
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
            var host = new RabbitMqHost(settings);
            _hosts.Add(host);

            if (_hosts.Count == 1 && string.IsNullOrWhiteSpace(_settings.QueueName))
                _settings.QueueName = _configuration.ConsumeTopology.CreateTemporaryQueueName("bus-");

            return host;
        }

        public string CreateTemporaryQueueName(string prefix)
        {
            return _configuration.ConsumeTopology.CreateTemporaryQueueName(prefix);
        }

        void IRabbitMqBusFactoryConfigurator.SendTopology<T>(Action<IRabbitMqMessageSendTopologyConfigurator<T>> configureTopology)
        {
            IRabbitMqMessageSendTopologyConfigurator<T> configurator = _configuration.SendTopology.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        void IRabbitMqBusFactoryConfigurator.PublishTopology<T>(Action<IRabbitMqMessagePublishTopologyConfigurator<T>> configureTopology)
        {
            IRabbitMqMessagePublishTopologyConfigurator<T> configurator = _configuration.PublishTopology.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void OverrideDefaultBusEndpointQueueName(string value)
        {
            _settings.ExchangeName = value;
            _settings.QueueName = value;
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_hosts.Count == 0)
                throw new ArgumentException("At least one host must be configured before configuring a receive endpoint");

            ReceiveEndpoint(_hosts[0], queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(IRabbitMqHost host, string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            if (host == null)
                throw new EndpointNotFoundException("The host address specified was not configured.");

            var endpointTopologySpecification = _configuration.CreateConfiguration();

            var specification = new RabbitMqReceiveEndpointSpecification(host, endpointTopologySpecification, queueName);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }
    }
}