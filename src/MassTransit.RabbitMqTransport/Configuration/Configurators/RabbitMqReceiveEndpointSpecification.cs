// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using Builders;
    using EndpointConfigurators;
    using GreenPipes;
    using Management;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using Topology;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointSpecification :
        ReceiveEndpointSpecification,
        IRabbitMqReceiveEndpointConfigurator,
        IReceiveEndpointSpecification<IBusBuilder>
    {
        readonly List<ExchangeBindingSettings> _exchangeBindings;
        readonly IRabbitMqHost _host;
        readonly IManagementPipe _managementPipe;
        readonly RabbitMqReceiveSettings _settings;
        bool _bindMessageExchanges;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;

        public RabbitMqReceiveEndpointSpecification(IRabbitMqHost host, string queueName = null, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _host = host;

            _bindMessageExchanges = true;

            _settings = new RabbitMqReceiveSettings
            {
                QueueName = queueName
            };

            _managementPipe = new ManagementPipe();
            _exchangeBindings = new List<ExchangeBindingSettings>();
        }

        public RabbitMqReceiveEndpointSpecification(IRabbitMqHost host, RabbitMqReceiveSettings settings, IConsumePipe consumePipe)
            : base(consumePipe)
        {
            _host = host;
            _settings = settings;

            _managementPipe = new ManagementPipe();
            _exchangeBindings = new List<ExchangeBindingSettings>();
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
            set { SetQueueArgument("x-queue-mode", value ? "lazy" : "default"); }
        }

        public bool BindMessageExchanges
        {
            set { _bindMessageExchanges = value; }
        }

        public void SetQueueArgument(string key, object value)
        {
            _settings.SetQueueArgument(key, value);
        }

        public void SetExchangeArgument(string key, object value)
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

            _exchangeBindings.AddRange(_settings.GetExchangeBindings(exchangeName));
        }

        public void Bind<T>()
            where T : class
        {
            _exchangeBindings.AddRange(typeof(T).GetExchangeBindings(_host.Settings.MessageNameFormatter));
        }

        public void Bind(string exchangeName, Action<IExchangeBindingConfigurator> callback)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var exchangeSettings = new RabbitMqReceiveSettings(_settings);

            callback(exchangeSettings);

            _exchangeBindings.AddRange(exchangeSettings.GetExchangeBindings(exchangeName));
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
            var receiveEndpointBuilder = new RabbitMqReceiveEndpointBuilder(CreateConsumePipe(builder), builder, _bindMessageExchanges, _host);

            receiveEndpointBuilder.AddExchangeBindings(_exchangeBindings.ToArray());

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            _sendEndpointProvider = CreateSendEndpointProvider(receiveEndpointBuilder);
            _publishEndpointProvider = CreatePublishEndpointProvider(receiveEndpointBuilder);

            var transport = new RabbitMqReceiveTransport(_host, _settings, _managementPipe, receiveEndpointBuilder.GetExchangeBindings().ToArray(),
                _sendEndpointProvider, _publishEndpointProvider);

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
            var errorQueueName = _settings.QueueName + "_error";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, RabbitMQ.Client.ExchangeType.Fanout, _settings.Durable,
                _settings.AutoDelete);

            sendSettings.BindToQueue(errorQueueName);

            return sendSettings.GetSendAddress(_host.Settings.HostAddress);
        }

        protected override Uri GetDeadLetterAddress()
        {
            var errorQueueName = _settings.QueueName + "_skipped";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, RabbitMQ.Client.ExchangeType.Fanout, _settings.Durable,
                _settings.AutoDelete);

            sendSettings.BindToQueue(errorQueueName);

            return sendSettings.GetSendAddress(_host.Settings.HostAddress);
        }
    }
}