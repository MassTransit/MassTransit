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
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;
    using Saga;
    using SagaConfigurators;
    using Transports;


    public abstract class ReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration
    {
        readonly IEndpointConfiguration _configuration;
        readonly Lazy<IConsumePipe> _consumePipe;
        readonly IHostConfiguration _hostConfiguration;
        readonly IList<string> _lateConfigurationKeys;
        readonly IList<IReceiveEndpointSpecification> _specifications;

        protected ReceiveEndpointConfiguration(IHostConfiguration hostConfiguration, IEndpointConfiguration configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;

            _consumePipe = new Lazy<IConsumePipe>(() => _configuration.Consume.CreatePipe());
            _specifications = new List<IReceiveEndpointSpecification>();
            _lateConfigurationKeys = new List<string>();

            EndpointObservers = new ReceiveEndpointObservable();
            ReceiveObservers = new ReceiveObservable();
            TransportObservers = new ReceiveTransportObservable();
        }

        public ReceiveEndpointObservable EndpointObservers { get; }
        public ReceiveObservable ReceiveObservers { get; }
        public ReceiveTransportObservable TransportObservers { get; }

        public IConsumePipeConfiguration Consume => _configuration.Consume;
        public ISendPipeConfiguration Send => _configuration.Send;
        public IPublishPipeConfiguration Publish => _configuration.Publish;
        public IReceivePipeConfiguration Receive => _configuration.Receive;

        public ITopologyConfiguration Topology => _configuration.Topology;

        public ISerializationConfiguration Serialization => _configuration.Serialization;

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configuration.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configuration.ConnectConsumerConfigurationObserver(observer);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configuration.ConnectSagaConfigurationObserver(observer);
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            _configuration.ConsumerConfigured(configurator);
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            _configuration.ConsumerMessageConfigured(configurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            _configuration.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _configuration.SagaMessageConfigured(configurator);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _configuration.ConnectHandlerConfigurationObserver(observer);
        }

        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            _configuration.HandlerConfigured(configurator);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return EndpointObservers.Connect(observer);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            _configuration.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configuration.AddPrePipeSpecification(specification);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configuration.ConfigureSend(callback);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            _configuration.ConfigurePublish(callback);
        }

        public void ConfigureReceive(Action<IReceivePipeConfigurator> callback)
        {
            _configuration.ConfigureReceive(callback);
        }

        public void ConfigureDeadLetter(Action<IPipeConfigurator<ReceiveContext>> callback)
        {
            _configuration.ConfigureDeadLetter(callback);
        }

        public void ConfigureError(Action<IPipeConfigurator<ExceptionReceiveContext>> callback)
        {
            _configuration.ConfigureError(callback);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _configuration.Validate()
                .Concat(_specifications.SelectMany(x => x.Validate()))
                .Concat(_lateConfigurationKeys.Select(x => this.Failure(x, "was modified after being used")));
        }

        public IConsumePipe ConsumePipe => _consumePipe.Value;

        public abstract Uri HostAddress { get; }
        public abstract Uri InputAddress { get; }

        public virtual IReceivePipe CreateReceivePipe()
        {
            return _configuration.Receive.CreatePipe(ConsumePipe, _configuration.Serialization.Deserializer);
        }

        public abstract IReceiveEndpoint Build();

        protected virtual IReceiveEndpoint CreateReceiveEndpoint(string endpointName, IReceiveTransport receiveTransport,
            ReceiveEndpointContext receiveEndpointContext)
        {
            var receiveEndpoint = new ReceiveEndpoint(receiveTransport, receiveEndpointContext);

            _hostConfiguration.Host.AddReceiveEndpoint(endpointName, receiveEndpoint);

            return receiveEndpoint;
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _configuration.Serialization.SetSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _configuration.Serialization.AddDeserializer(contentType, deserializerFactory);
        }

        public void ClearMessageDeserializers()
        {
            _configuration.Serialization.ClearDeserializers();
        }

        protected void ApplySpecifications(IReceiveEndpointBuilder builder)
        {
            for (var i = 0; i < _specifications.Count; i++)
                _specifications[i].Configure(builder);
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification specification)
        {
            _specifications.Add(specification);
        }

        protected void Changed(string key)
        {
            if (IsAlreadyConfigured())
                _lateConfigurationKeys.Add(key);
        }

        protected virtual bool IsAlreadyConfigured()
        {
            return false;
        }
    }
}
