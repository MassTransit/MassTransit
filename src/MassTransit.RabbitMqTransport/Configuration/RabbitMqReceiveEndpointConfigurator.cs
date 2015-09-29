// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqReceiveEndpointConfigurator :
        ReceiveEndpointConfigurator,
        IRabbitMqReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IRabbitMqHost _host;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqReceiveEndpointConfigurator(IRabbitMqHost host, string queueName = null, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _host = host;

            _settings = new RabbitMqReceiveSettings
            {
                QueueName = queueName,
                ExchangeName = queueName
            };
        }

        public RabbitMqReceiveEndpointConfigurator(IRabbitMqHost host, RabbitMqReceiveSettings settings, IConsumePipe consumePipe)
            : base(consumePipe)
        {
            _host = host;

            _settings = settings;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (ValidationResult result in base.Validate())
                yield return result.WithParentKey($"{_settings.QueueName}");

            if (!RabbitMqAddressExtensions.IsValidQueueName(_settings.QueueName))
                yield return this.Failure($"{_settings.QueueName}", "Is not a valid queue name");
            if (_settings.PurgeOnStartup)
                yield return this.Warning($"{_settings.QueueName}", "Existing messages in the queue will be purged on service start");
        }

        public Uri InputAddress => _host.Settings.GetInputAddress(_settings);

        public void Apply(IBusBuilder builder)
        {
            RabbitMqReceiveEndpointBuilder endpointBuilder = null;
            IPipe<ReceiveContext> receivePipe = CreateReceivePipe(builder, consumePipe =>
            {
                endpointBuilder = new RabbitMqReceiveEndpointBuilder(consumePipe, _host.MessageNameFormatter);
                return endpointBuilder;
            });

            if (endpointBuilder == null)
                throw new InvalidOperationException("The endpoint builder was not initialized");

            var transport = new RabbitMqReceiveTransport(_host.ConnectionCache, _settings, endpointBuilder.GetExchangeBindings().ToArray());

            builder.AddReceiveEndpoint(new ReceiveEndpoint(transport, receivePipe));
        }

        public bool Durable
        {
            set { _settings.Durable = value; }
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

        public ushort PrefetchCount
        {
            set { _settings.PrefetchCount = value; }
        }

        public void SetQueueArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _settings.QueueArguments.Remove(key);
            else
                _settings.QueueArguments[key] = value;
        }

        public void SetExchangeArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _settings.ExchangeArguments.Remove(key);
            else
                _settings.ExchangeArguments[key] = value;
        }

        protected override Uri GetErrorAddress()
        {
            string errorQueueName = _settings.QueueName + "_error";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, RabbitMQ.Client.ExchangeType.Fanout, _settings.Durable,
                _settings.AutoDelete);

            sendSettings.BindToQueue(errorQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
        }

        protected override Uri GetDeadLetterAddress()
        {
            string errorQueueName = _settings.QueueName + "_skipped";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, RabbitMQ.Client.ExchangeType.Fanout, _settings.Durable,
                _settings.AutoDelete);

            sendSettings.BindToQueue(errorQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
        }
    }
}