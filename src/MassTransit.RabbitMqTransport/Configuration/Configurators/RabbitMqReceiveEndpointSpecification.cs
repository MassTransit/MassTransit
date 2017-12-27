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
    using EndpointSpecifications;
    using GreenPipes;
    using Management;
    using MassTransit.Builders;
    using MassTransit.Pipeline.Pipes;
    using Specifications;
    using Topology;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointSpecification :
        ReceiveEndpointSpecification,
        IRabbitMqReceiveEndpointConfigurator,
        IReceiveEndpointSpecification<IBusBuilder>
    {
        readonly IRabbitMqHost _host;
        readonly IManagementPipe _managementPipe;
        readonly RabbitMqReceiveSettings _settings;
        readonly IRabbitMqEndpointConfiguration _configuration;
        bool _bindMessageExchanges;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;

        public RabbitMqReceiveEndpointSpecification(IRabbitMqHost host, IRabbitMqEndpointConfiguration configuration, string queueName = null)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;

            _bindMessageExchanges = true;

            _settings = new RabbitMqReceiveSettings(queueName, configuration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false)
            {
                QueueName = queueName
            };

            _managementPipe = new ManagementPipe();
        }

        public RabbitMqReceiveEndpointSpecification(IRabbitMqHost host, IRabbitMqEndpointConfiguration configuration,
            RabbitMqReceiveSettings settings)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
            _settings = settings;

            _managementPipe = new ManagementPipe();
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider;

        IRabbitMqHost IRabbitMqReceiveEndpointConfigurator.Host => _host;

        public bool Durable
        {
            set
            {
                _settings.Durable = value;

                Changed("Durable");
            }
        }

        public string QueueName
        {
            set { _settings.QueueName = value; }
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
            set { _settings.ExchangeType = value; }
        }

        public bool PurgeOnStartup
        {
            set { _settings.PurgeOnStartup = value; }
        }

        public ushort PrefetchCount
        {
            set
            {
                _settings.PrefetchCount = value;

                Changed("PrefetchCount");
            }
        }

        public bool Lazy
        {
            set { _settings.Lazy = value; }
        }

        public bool BindMessageExchanges
        {
            set { _bindMessageExchanges = value; }
        }

        public string DeadLetterExchange
        {
            set { SetQueueArgument("x-dead-letter-exchange", value); }
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

        public void Bind(string exchangeName)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            _configuration.Topology.Consume.Bind(exchangeName);
        }

        public void Bind<T>()
            where T : class
        {
            _configuration.Topology.Consume.GetMessageTopology<T>().Bind();
        }

        public void Bind(string exchangeName, Action<IExchangeBindingConfigurator> callback)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _configuration.Topology.Consume.Bind(exchangeName, callback);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result.WithParentKey($"{_settings.QueueName}");

            if (!RabbitMqAddressExtensions.IsValidQueueName(_settings.QueueName))
                yield return this.Failure($"{_settings.QueueName}", "Is not a valid queue name");
            if (_settings.PurgeOnStartup)
                yield return this.Warning($"{_settings.QueueName}", "Existing messages in the queue will be purged on service start");
        }

        public void Apply(IBusBuilder builder)
        {
            var rabbitMqBusBuilder = builder as RabbitMqBusBuilder;
            if (rabbitMqBusBuilder == null)
                throw new ConfigurationException("Must be a RabbitMqBusBuilder");

            var receiveEndpointBuilder = new RabbitMqReceiveEndpointBuilder(builder, _host, rabbitMqBusBuilder.Hosts, _bindMessageExchanges, _configuration);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            _sendEndpointProvider = receiveEndpointTopology.SendEndpointProvider;
            _publishEndpointProvider = receiveEndpointTopology.PublishEndpointProvider;

            var transport = new RabbitMqReceiveTransport(_host, _settings, _managementPipe, receiveEndpointTopology);

            var rabbitMqHost = _host as RabbitMqHost;
            if (rabbitMqHost == null)
                throw new ConfigurationException("Must be a RabbitMqHost");

            rabbitMqHost.ReceiveEndpoints.Add(_settings.QueueName ?? NewId.Next().ToString(), new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetInputAddress()
        {
            return _settings.GetInputAddress(_host.Settings.HostAddress);
        }

        protected override Uri GetErrorAddress()
        {
            return _configuration.Topology.Send.GetErrorAddress(_settings, _host.Address);
        }

        protected override Uri GetDeadLetterAddress()
        {
            return _configuration.Topology.Send.GetDeadLetterAddress(_settings, _host.Address);
        }
    }
}