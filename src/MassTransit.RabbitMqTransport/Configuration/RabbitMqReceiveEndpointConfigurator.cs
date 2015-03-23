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
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Builders;
    using EndpointConfigurators;
    using Integration;
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Pipeline.Pipes;
    using PipeConfigurators;
    using Pipeline;
    using Policies;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqReceiveEndpointConfigurator :
        IRabbitMqReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IList<IReceiveEndpointSpecification> _configurators;
        readonly IList<IPipeSpecification<ConsumeContext>> _consumePipeSpecifications;
        readonly IRabbitMqHost _host;
        readonly IBuildPipeConfigurator<ReceiveContext> _receivePipeConfigurator;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqReceiveEndpointConfigurator(IRabbitMqHost host, string queueName = null)
        {
            _host = host;
            _consumePipeSpecifications = new List<IPipeSpecification<ConsumeContext>>();
            _receivePipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _configurators = new List<IReceiveEndpointSpecification>();

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

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurators.SelectMany(x => x.Validate());
        }

        public void Configure(IBusBuilder builder)
        {
            ReceiveEndpoint receiveEndpoint = CreateReceiveEndpoint(builder.MessageDeserializer);

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

        public void AddConfigurator(IReceiveEndpointSpecification configurator)
        {
            _configurators.Add(configurator);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecifications.Add(specification);
        }

        ReceiveEndpoint CreateReceiveEndpoint(IMessageDeserializer deserializer)
        {
            IRetryPolicy retryPolicy = Retry.Exponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1));

            var consumePipe = new ConsumePipe(_consumePipeSpecifications);


            var builder = new RabbitMqReceiveEndpointBuilder(consumePipe, _host.MessageNameFormatter);

            foreach (IReceiveEndpointSpecification builderConfigurator in _configurators)
                builderConfigurator.Configure(builder);

            var modelCache = new RabbitMqModelCache(_host.SendConnectionCache);

            string errorQueueName = _settings.QueueName + "_error";
            var sendSettings = new RabbitMqSendSettings(errorQueueName, RabbitMQ.Client.ExchangeType.Fanout, true,
                false);

            var sendTransport = new RabbitMqSendTransport(modelCache, sendSettings);

            var errorSettings = new RabbitMqErrorQueueSettings
            {
                QueueName = errorQueueName,
                ExchangeName = errorQueueName,
                AutoDelete = sendSettings.AutoDelete,
                Durable = sendSettings.Durable
            };
            sendTransport.AddModelFilter(new PrepareErrorQueueFilter(errorSettings));

            IPipe<ReceiveContext> moveToErrorPipe = Pipe.New<ReceiveContext>(
                x => x.Filter(new MoveToErrorTransportFilter(() => Task.FromResult<ISendTransport>(sendTransport))));

            _receivePipeConfigurator.Rescue(moveToErrorPipe, typeof(SerializationException));


            _receivePipeConfigurator.Filter(new DeserializeFilter(deserializer, consumePipe));

            IPipe<ReceiveContext> receivePipe = _receivePipeConfigurator.Build();


            var transport = new RabbitMqReceiveTransport(_host.ConnectionCache, _settings,
                inputAddress, Retry.None, builder.GetExchangeBindings().ToArray());

            return new ReceiveEndpoint(transport, receivePipe, consumePipe);
        }
    }
}