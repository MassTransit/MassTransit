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
namespace MassTransit.RabbitMqTransport.EndpointSpecifications
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Management;
    using MassTransit.Builders;
    using MassTransit.EndpointSpecifications;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Pipeline.Pipes;
    using Pipeline;
    using Topology;
    using Topology.Settings;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointSpecification :
        ReceiveEndpointSpecification,
        IRabbitMqReceiveEndpointConfigurator,
        IReceiveEndpointSpecification<IBusBuilder>
    {
        readonly IRabbitMqEndpointConfiguration _configuration;
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionContextConfigurator;
        readonly RabbitMqHost _host;
        readonly IManagementPipe _managementPipe;
        readonly IBuildPipeConfigurator<ModelContext> _modelContextConfigurator;
        readonly RabbitMqReceiveSettings _settings;
        bool _bindMessageExchanges;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;

        public RabbitMqReceiveEndpointSpecification(RabbitMqHost host, IRabbitMqEndpointConfiguration configuration, string queueName = null)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
            _settings = new RabbitMqReceiveSettings(queueName, configuration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false)
            {
                QueueName = queueName
            };

            _bindMessageExchanges = true;

            _managementPipe = new ManagementPipe();

            _connectionContextConfigurator = new PipeConfigurator<ConnectionContext>();
            _modelContextConfigurator = new PipeConfigurator<ModelContext>();
        }

        public RabbitMqReceiveEndpointSpecification(IRabbitMqHost host, IRabbitMqEndpointConfiguration configuration,
            RabbitMqReceiveSettings settings)
            : base(configuration)
        {
            _host = host as RabbitMqHost ?? throw new ArgumentException("The host must be a RabbitMqHost", nameof(host));
            _configuration = configuration;
            _settings = settings;

            _managementPipe = new ManagementPipe();

            _connectionContextConfigurator = new PipeConfigurator<ConnectionContext>();
            _modelContextConfigurator = new PipeConfigurator<ModelContext>();
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
            set => _settings.QueueName = value;
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
            set => _settings.Lazy = value;
        }

        public bool BindMessageExchanges
        {
            set => _bindMessageExchanges = value;
        }

        public string DeadLetterExchange
        {
            set => SetQueueArgument("x-dead-letter-exchange", value);
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

            var receiveEndpointBuilder = new RabbitMqReceiveEndpointBuilder(_host, rabbitMqBusBuilder.Hosts, _bindMessageExchanges, _configuration);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            _sendEndpointProvider = receiveEndpointTopology.SendEndpointProvider;
            _publishEndpointProvider = receiveEndpointTopology.PublishEndpointProvider;

            _modelContextConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointTopology.BrokerTopology));

            if (_settings.PurgeOnStartup)
                _modelContextConfigurator.UseFilter(new PurgeOnStartupFilter(_settings.QueueName));

            _modelContextConfigurator.UseFilter(new PrefetchCountFilter(_managementPipe, _settings.PrefetchCount));

            var deadLetterTransport = CreateDeadLetterTransport();

            var errorTransport = CreateErrorTransport();

            var receiveObserver = new ReceiveObservable();
            var transportObserver = new ReceiveTransportObservable();

            var consumerFilter = new RabbitMqConsumerFilter(receivePipe, receiveObserver, transportObserver, receiveEndpointTopology, deadLetterTransport, errorTransport);
            _modelContextConfigurator.UseFilter(consumerFilter);

            IFilter<ConnectionContext> modelFilter = new ReceiveModelFilter(_modelContextConfigurator.Build(), _host);

            _connectionContextConfigurator.UseFilter(modelFilter);

            var transport = new RabbitMqReceiveTransport(_host, _settings, _connectionContextConfigurator.Build(), receiveEndpointTopology, receiveObserver,
                transportObserver);

            transport.Add(consumerFilter);

            _host.ReceiveEndpoints.Add(_settings.QueueName ?? NewId.Next().ToString(), new ReceiveEndpoint(transport, receivePipe));
        }

        IErrorTransport CreateErrorTransport()
        {
            var settings = _configuration.Topology.Send.GetErrorSettings(_settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(settings, settings.GetBrokerTopology());

            return new RabbitMqErrorTransport(settings.ExchangeName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var settings = _configuration.Topology.Send.GetDeadLetterSettings(_settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(settings, settings.GetBrokerTopology());

            return new RabbitMqDeadLetterTransport(settings.ExchangeName, filter);
        }

        protected override Uri GetInputAddress()
        {
            return _settings.GetInputAddress(_host.Settings.HostAddress);
        }
    }
}