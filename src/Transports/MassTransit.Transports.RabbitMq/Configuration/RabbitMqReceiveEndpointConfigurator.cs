// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EndpointConfigurators;
    using Magnum.Extensions;
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using PipeConfigurators;
    using Policies;
    using RabbitMQ.Client;


    public class RabbitMqReceiveEndpointConfigurator :
        IRabbitMqReceiveEndpointConfigurator,
        IServiceBusFactoryBuilderConfigurator
    {
        readonly IList<IReceiveEndpointBuilderConfigurator> _configurators;
        readonly RabbitMqHostSettings _hostSettings;
        readonly PipeConfigurator<ConsumeContext> _pipeConfigurator;
        readonly PipeConfigurator<ReceiveContext> _receivePipeConfigurator;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqReceiveEndpointConfigurator(RabbitMqHostSettings hostSettings, string queueName = null)
        {
            _hostSettings = hostSettings;
            _pipeConfigurator = new PipeConfigurator<ConsumeContext>();
            _receivePipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _configurators = new List<IReceiveEndpointBuilderConfigurator>();

            _settings = new RabbitMqReceiveSettings
            {
                QueueName = queueName,
                ExchangeName = queueName,
            };
        }

        public ReceiveSettings Settings
        {
            get { return _settings; }
        }

        public void Durable(bool durable = true)
        {
            _settings.Durable = durable;
        }

        public void Exclusive(bool exclusive = true)
        {
            _settings.Exclusive = exclusive;
        }

        public void AutoDelete(bool autoDelete = true)
        {
            _settings.AutoDelete = autoDelete;
        }

        public void PurgeOnStartup(bool purgeOnStartup = true)
        {
            _settings.PurgeOnStartup = purgeOnStartup;
        }

        public void PrefetchCount(ushort limit)
        {
            _settings.PrefetchCount = limit;
        }

        public void ExchangeType(ExchangeType exchangeType)
        {
            _settings.ExchangeType = exchangeType.ToString();
        }

        public void SetQueueArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            _settings.QueueArguments[key] = value;
        }

        public void SetExchangeArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            _settings.ExchangeArguments[key] = value;
        }

        public void AddConfigurator(IReceiveEndpointBuilderConfigurator configurator)
        {
            _configurators.Add(configurator);
        }

        public void AddPipeBuilderConfigurator(IPipeBuilderConfigurator<ConsumeContext> configurator)
        {
            _pipeConfigurator.AddPipeBuilderConfigurator(configurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurators.SelectMany(x => x.Validate());
        }

        public void Configure(IServiceBusBuilder builder)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _hostSettings.Host,
                Port = _hostSettings.Port,
                VirtualHost = _hostSettings.VirtualHost,
                UserName = _hostSettings.Username,
                Password = _hostSettings.Password,
                RequestedHeartbeat = _hostSettings.Heartbeat
            };

            ReceiveEndpoint receiveEndpoint = CreateReceiveEndpoint(connectionFactory, builder.MessageDeserializer);

            builder.AddReceiveEndpoint(receiveEndpoint);
        }

        ReceiveEndpoint CreateReceiveEndpoint(ConnectionFactory connectionFactory, IMessageDeserializer deserializer)
        {
            IRetryPolicy retryPolicy = Retry.Exponential(1.Seconds(), 10.Seconds(), 1.Seconds());

            var connectionMaker = new RabbitMqConnector(connectionFactory, retryPolicy);

            var transport = new RabbitMqReceiveTransport(connectionMaker, Retry.None, _settings);

            var inboundPipe = new InboundPipe(_pipeConfigurator);

            IReceiveEndpointBuilder builder = new ReceiveEndpointBuilder(inboundPipe);

            foreach (IReceiveEndpointBuilderConfigurator builderConfigurator in _configurators)
                builderConfigurator.Configure(builder);

            _receivePipeConfigurator.Filter(new DeserializeFilter(deserializer, inboundPipe));

            IPipe<ReceiveContext> receivePipe = _receivePipeConfigurator.Build();

            return new ReceiveEndpoint(transport, receivePipe);
        }
    }
}