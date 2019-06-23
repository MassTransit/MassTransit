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
namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using Builders;
    using Configuration;
    using ConsumeConfigurators;
    using Context;
    using EndpointConfigurators;
    using EndpointSpecifications;
    using GreenPipes;
    using Pipeline.Observables;
    using Saga;
    using SagaConfigurators;
    using Topology;


    public abstract class BusFactoryConfigurator :
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator,
        IEndpointConfigurationObserverConnector
    {
        readonly IEndpointConfiguration _busEndpointConfiguration;
        readonly IBusConfiguration _configuration;
        readonly IList<IReceiveEndpointSpecification<IBusBuilder>> _endpointSpecifications;
        readonly IList<IBusFactorySpecification> _specifications;

        protected BusFactoryConfigurator(IBusConfiguration configuration, IEndpointConfiguration busEndpointConfiguration)
        {
            _configuration = configuration;
            _busEndpointConfiguration = busEndpointConfiguration;

            BusObservable = new BusObservable();
            EndpointObservable = new EndpointConfigurationObservable();

            _specifications = new List<IBusFactorySpecification>();
            _endpointSpecifications = new List<IReceiveEndpointSpecification<IBusBuilder>>();

            if (LogContext.Current == null)
                LogContext.ConfigureCurrentLogContext();
        }

        protected BusObservable BusObservable { get; }
        protected EndpointConfigurationObservable EndpointObservable { get; }

        public IMessageTopologyConfigurator MessageTopology => _configuration.Topology.Message;
        public ISendTopologyConfigurator SendTopology => _configuration.Topology.Send;
        public IPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return BusObservable.Connect(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return EndpointObservable.Connect(observer);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configuration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configuration.Consume.Configurator.AddPrePipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            _configuration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_configuration.Send.Configurator);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_configuration.Publish.Configurator);
        }

        public void Message<T>(Action<IMessageTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessageTopologyConfigurator<T> configurator = _configuration.Topology.Message.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessageSendTopologyConfigurator<T> configurator = _configuration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessagePublishTopologyConfigurator<T> configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void AddBusFactorySpecification(IBusFactorySpecification specification)
        {
            _specifications.Add(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _busEndpointConfiguration.Consume.Configurator.ConnectConsumerConfigurationObserver(observer);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _busEndpointConfiguration.Consume.Configurator.ConnectSagaConfigurationObserver(observer);
        }

        public abstract void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null);

        public abstract void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint);

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate())
                .Concat(_endpointSpecifications.SelectMany(x => x.Validate()))
                .Concat(_busEndpointConfiguration.Validate())
                .Concat(_configuration.Validate());
        }

        protected void ApplySpecifications(IBusBuilder builder)
        {
            foreach (IBusFactorySpecification configurator in _specifications)
                configurator.Apply(builder);

            foreach (IReceiveEndpointSpecification<IBusBuilder> configurator in _endpointSpecifications)
                configurator.Apply(builder);
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

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            _busEndpointConfiguration.Consume.Configurator.ConsumerConfigured(configurator);
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            _busEndpointConfiguration.Consume.Configurator.ConsumerMessageConfigured(configurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            _busEndpointConfiguration.Consume.Configurator.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _busEndpointConfiguration.Consume.Configurator.SagaMessageConfigured(configurator);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _busEndpointConfiguration.Consume.Configurator.ConnectHandlerConfigurationObserver(observer);
        }

        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            _busEndpointConfiguration.Consume.Configurator.HandlerConfigured(configurator);
        }

        protected void ConfigureReceiveEndpoint<T>(IReceiveEndpointConfiguration configuration, T configurator,
            Action<T> configure)
            where T : IReceiveEndpointConfigurator
        {
            configuration.ConnectConsumerConfigurationObserver(this);
            configuration.ConnectSagaConfigurationObserver(this);
            configuration.ConnectHandlerConfigurationObserver(this);

            configure?.Invoke(configurator);

            EndpointObservable.EndpointConfigured(configurator);

            var specification = new ConfigurationReceiveEndpointSpecification(configuration);

            _endpointSpecifications.Add(specification);
        }
    }
}
