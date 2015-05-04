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
    using BusConfigurators;
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using PipeConfigurators;
    using Transports;


    public class RabbitMqReceiveEndpointConfigurator :
        IRabbitMqReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IList<IReceiveEndpointSpecification> _configurators;
        readonly IConsumePipe _consumePipe;
        readonly ConsumePipeSpecification _consumePipeSpecification;
        readonly IRabbitMqHost _host;
        readonly IBuildPipeConfigurator<ReceiveContext> _receivePipeConfigurator;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqReceiveEndpointConfigurator(IRabbitMqHost host, string queueName = null, IConsumePipe consumePipe = null)
        {
            _host = host;
            _consumePipe = consumePipe;
            _consumePipeSpecification = new ConsumePipeSpecification();
            _receivePipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _configurators = new List<IReceiveEndpointSpecification>();

            _settings = new RabbitMqReceiveSettings
            {
                QueueName = queueName,
                ExchangeName = queueName,
            };
        }

        public Uri InputAddress
        {
            get { return _host.Settings.GetInputAddress(_settings); }
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurators.SelectMany(x => x.Validate());
        }

        public void Apply(IBusBuilder builder)
        {
            ReceiveEndpoint receiveEndpoint = CreateReceiveEndpoint(builder);

            builder.AddReceiveEndpoint(receiveEndpoint);
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

        public ushort PrefetchCount
        {
            set { _settings.PrefetchCount = value; }
        }

        public void ExchangeType(string exchangeType)
        {
            _settings.ExchangeType = exchangeType;
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

        public void AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _configurators.Add(configurator);
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecification.Add(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _consumePipeSpecification.Add(specification);
        }

        ReceiveEndpoint CreateReceiveEndpoint(IBusBuilder builder)
        {
            IConsumePipe consumePipe = _consumePipe ?? builder.CreateConsumePipe(_consumePipeSpecification);

            var endpointBuilder = new RabbitMqReceiveEndpointBuilder(consumePipe, _host.MessageNameFormatter);

            foreach (IReceiveEndpointSpecification builderConfigurator in _configurators)
                builderConfigurator.Configure(endpointBuilder);

            string errorQueueName = _settings.QueueName + "_error";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, RabbitMQ.Client.ExchangeType.Fanout, true,
                false);

            sendSettings.BindToQueue(errorQueueName);

            Uri errorQueueAddress = _host.Settings.GetSendAddress(sendSettings);

            ISendTransportProvider sendTransportProvider = builder.SendTransportProvider;

            IPipe<ReceiveContext> moveToErrorPipe = Pipe.New<ReceiveContext>(
                x => x.Filter(new MoveToErrorTransportFilter(() => sendTransportProvider.GetSendTransport(errorQueueAddress))));

            _receivePipeConfigurator.Rescue(moveToErrorPipe, typeof(Exception));

            _receivePipeConfigurator.Filter(new DeserializeFilter(builder.MessageDeserializer, consumePipe));

            IPipe<ReceiveContext> receivePipe = _receivePipeConfigurator.Build();

            var transport = new RabbitMqReceiveTransport(_host.ConnectionCache, _settings,
                endpointBuilder.GetExchangeBindings().ToArray());

            return new ReceiveEndpoint(transport, receivePipe);
        }
    }
}