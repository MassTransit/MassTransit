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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Management;
    using MassTransit.Configuration;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Pipeline.Pipes;
    using Pipeline;
    using Topology;
    using Topology.Settings;
    using Transport;


    public class RabbitMqReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IRabbitMqReceiveEndpointConfiguration,
        IRabbitMqReceiveEndpointConfigurator
    {
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionConfigurator;
        readonly IRabbitMqEndpointConfiguration _endpointConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly IManagementPipe _managementPipe;
        readonly IBuildPipeConfigurator<ModelContext> _modelConfigurator;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqReceiveEndpointConfiguration(IRabbitMqHostConfiguration hostConfiguration, RabbitMqReceiveSettings settings,
            IRabbitMqEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _settings = settings;

            HostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;

            BindMessageExchanges = true;

            _managementPipe = new ManagementPipe();
            _connectionConfigurator = new PipeConfigurator<ConnectionContext>();
            _modelConfigurator = new PipeConfigurator<ModelContext>();

            HostAddress = hostConfiguration.HostAddress;

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public IRabbitMqReceiveEndpointConfigurator Configurator => this;

        public IRabbitMqBusConfiguration BusConfiguration => HostConfiguration.BusConfiguration;
        public IRabbitMqHostConfiguration HostConfiguration { get; }

        public bool BindMessageExchanges { get; set; }

        public ReceiveSettings Settings => _settings;

        public override Uri HostAddress { get; }

        public override Uri InputAddress => _inputAddress.Value;

        IRabbitMqTopologyConfiguration IRabbitMqEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override IReceiveEndpoint Build()
        {
            var builder = new RabbitMqReceiveEndpointBuilder(this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            _modelConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology));

            IAgent consumerAgent;
            if (HostConfiguration.BusConfiguration.DeployTopologyOnly)
            {
                var transportReadyFilter = new TransportReadyFilter<ModelContext>(receiveEndpointContext);
                _modelConfigurator.UseFilter(transportReadyFilter);

                consumerAgent = transportReadyFilter;
            }
            else
            {
                if (_settings.PurgeOnStartup)
                    _modelConfigurator.UseFilter(new PurgeOnStartupFilter(_settings.QueueName));

                _modelConfigurator.UseFilter(new PrefetchCountFilter(_managementPipe, _settings.PrefetchCount));

                var consumerFilter = new RabbitMqConsumerFilter(receiveEndpointContext);

                _modelConfigurator.UseFilter(consumerFilter);

                consumerAgent = consumerFilter;
            }

            IFilter<ConnectionContext> modelFilter = new ReceiveEndpointFilter(_modelConfigurator.Build());

            _connectionConfigurator.UseFilter(modelFilter);

            var transport = new RabbitMqReceiveTransport(HostConfiguration.Host, _settings, _connectionConfigurator.Build(), receiveEndpointContext);

            transport.Add(consumerAgent);

            return CreateReceiveEndpoint(_settings.QueueName ?? NewId.Next().ToString(), transport, receiveEndpointContext);
        }

        public bool Durable
        {
            set
            {
                _settings.Durable = value;

                Changed("Durable");
            }
        }

        public bool Exclusive
        {
            set
            {
                _settings.Exclusive = value;

                Changed("Exclusive");
            }
        }

        public bool AutoDelete
        {
            set
            {
                _settings.AutoDelete = value;

                Changed("AutoDelete");
            }
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

        public ushort PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public bool Lazy
        {
            set => _settings.Lazy = value;
        }

        public bool BindQueue
        {
            set => _settings.BindQueue = value;
        }

        public TimeSpan? QueueExpiration
        {
            set => _settings.QueueExpiration = value;
        }

        public string DeadLetterExchange
        {
            set => SetQueueArgument(RabbitMQ.Client.Headers.XDeadLetterExchange, value);
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

        public void ConnectManagementEndpoint(IManagementEndpointConfigurator management)
        {
            var consumer = new SetPrefetchCountManagementConsumer(_managementPipe, _settings.QueueName);
            management.Instance(consumer);
        }

        public void Bind(string exchangeName, Action<IExchangeBindingConfigurator> callback)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            _endpointConfiguration.Topology.Consume.Bind(exchangeName, callback);
        }

        public void Bind<T>(Action<IExchangeBindingConfigurator> callback)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Bind(callback);
        }

        public void BindDeadLetterQueue(string exchangeName, string queueName, Action<IQueueBindingConfigurator> configure)
        {
            _endpointConfiguration.Topology.Consume.BindQueue(exchangeName, queueName, configure);

            DeadLetterExchange = exchangeName;
        }

        public void ConfigureModel(Action<IPipeConfigurator<ModelContext>> configure)
        {
            configure?.Invoke(_modelConfigurator);
        }

        public void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure)
        {
            configure?.Invoke(_connectionConfigurator);
        }

        Uri FormatInputAddress()
        {
            return _settings.GetInputAddress(HostConfiguration.Host.Settings.HostAddress);
        }

        protected override bool IsAlreadyConfigured()
        {
            return _inputAddress.IsValueCreated || base.IsAlreadyConfigured();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            var queueName = $"{_settings.QueueName}";

            if (!RabbitMqEntityNameValidator.Validator.IsValidEntityName(_settings.QueueName))
                yield return this.Failure(queueName, "must be a valid queue name");

            if (_settings.PurgeOnStartup)
                yield return this.Warning(queueName, "Existing messages in the queue will be purged on service start");

            foreach (var result in base.Validate())
                yield return result.WithParentKey(queueName);
        }
    }
}
