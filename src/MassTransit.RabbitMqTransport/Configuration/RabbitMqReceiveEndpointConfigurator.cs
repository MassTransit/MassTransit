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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using EndpointConfigurators;
    using GreenPipes;
    using Management;
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using Topology;
    using Transports;


    public class RabbitMqReceiveEndpointConfigurator :
        ReceiveEndpointConfigurator,
        IRabbitMqReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly List<ExchangeBindingSettings> _exchangeBindings;
        readonly IRabbitMqHost _host;
        readonly IManagementPipe _managementPipe;
        readonly RabbitMqReceiveSettings _settings;
        bool _bindMessageExchanges;

        public RabbitMqReceiveEndpointConfigurator(IRabbitMqHost host, string queueName = null, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _host = host;

            _bindMessageExchanges = true;

            _settings = new RabbitMqReceiveSettings
            {
                QueueName = queueName,
                ExchangeTypeProvider = _host.Settings.ExchangeTypeProvider ?? new MasstransitExchangeTypeProvider(),
                RoutingKeyFormatter = _host.Settings.RoutingKeyFormatter ?? new MasstransitRoutingKeyFormatter()
            };

            _managementPipe = new ManagementPipe();
            _exchangeBindings = new List<ExchangeBindingSettings>();
        }

        public RabbitMqReceiveEndpointConfigurator(IRabbitMqHost host, RabbitMqReceiveSettings settings, IConsumePipe consumePipe)
            : base(consumePipe)
        {
            _host = host;

            _settings = settings;

            _managementPipe = new ManagementPipe();
            _exchangeBindings = new List<ExchangeBindingSettings>();
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
            RabbitMqReceiveEndpointBuilder endpointBuilder = null;
            var receivePipe = CreateReceivePipe(builder, consumePipe =>
            {
                endpointBuilder = new RabbitMqReceiveEndpointBuilder(consumePipe, _host.Settings.MessageNameFormatter, _host.Settings.ExchangeTypeProvider, _host.Settings.RoutingKeyFormatter, _bindMessageExchanges);

                endpointBuilder.AddExchangeBindings(_exchangeBindings.ToArray());

                return endpointBuilder;
            });

            if (endpointBuilder == null)
                throw new InvalidOperationException("The endpoint builder was not initialized");

            var transport = new RabbitMqReceiveTransport(_host, _settings, _managementPipe, endpointBuilder.GetExchangeBindings().ToArray());

            builder.AddReceiveEndpoint(_settings.QueueName ?? NewId.Next().ToString(), new ReceiveEndpoint(transport, receivePipe));
        }

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

        public IExchangeTypeProvider ExchangeTypeProvider { get; set; }

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

        /// <summary>
        /// Do not use this method when you want to make use of the configurable routing extension points since this method will add bindings without a routingkey
        /// </summary>
        public void Bind(string exchangeName)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            _exchangeBindings.AddRange(_settings.GetExchangeBindings(exchangeName));
        }

        public void Bind<T>()
            where T : class
        {
            _exchangeBindings.AddRange(typeof(T).GetExchangeBindings(_host.Settings.MessageNameFormatter, _host.Settings.ExchangeTypeProvider, _host.Settings.RoutingKeyFormatter));
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

        protected override Uri GetInputAddress()
        {
            return _host.Settings.GetInputAddress(_settings);
        }

        protected override Uri GetErrorAddress()
        {
            var errorQueueName = _settings.QueueName + "_error";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, _settings.Durable,
                _settings.AutoDelete, _host.Settings.ExchangeTypeProvider, _host.Settings.RoutingKeyFormatter);

            sendSettings.BindToQueue(errorQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
        }

        protected override Uri GetDeadLetterAddress()
        {
            var errorQueueName = _settings.QueueName + "_skipped";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, _settings.Durable,
                _settings.AutoDelete, _host.Settings.ExchangeTypeProvider, _host.Settings.RoutingKeyFormatter);

            sendSettings.BindToQueue(errorQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
        }
    }
}