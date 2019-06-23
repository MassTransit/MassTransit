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
namespace MassTransit.EndpointConfigurators
{
    using System;
    using System.Net.Mime;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using SagaConfigurators;
    using Transports;


    /// <summary>
    /// This is simply a delegate to the endpoint configurator for a management endpoint.
    /// A management endpoint is just a type of receive endpoint that can be used to communicate
    /// with middleware, etc.
    /// </summary>
    public class ManagementEndpointConfigurator :
        IManagementEndpointConfigurator
    {
        readonly IReceiveEndpointConfigurator _configurator;

        public ManagementEndpointConfigurator(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPrePipeSpecification(specification);
        }

        ConnectHandle IConsumerConfigurationObserverConnector.ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configurator.ConnectConsumerConfigurationObserver(observer);
        }

        void IReceiveEndpointConfigurator.AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _configurator.AddEndpointSpecification(configurator);
        }

        void IReceiveEndpointConfigurator.SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _configurator.SetMessageSerializer(serializerFactory);
        }

        void IReceiveEndpointConfigurator.AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _configurator.AddMessageDeserializer(contentType, deserializerFactory);
        }

        public void ClearMessageDeserializers()
        {
            _configurator.ClearMessageDeserializers();
        }

        Uri IReceiveEndpointConfigurator.InputAddress => _configurator.InputAddress;

        void ISendPipelineConfigurator.ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        void IPublishPipelineConfigurator.ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            _configurator.ConfigurePublish(callback);
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
            _configurator.ConsumerConfigured(configurator);
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            _configurator.ConsumerMessageConfigured(configurator);
        }

        ConnectHandle ISagaConfigurationObserverConnector.ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configurator.ConnectSagaConfigurationObserver(observer);
        }

        void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
        {
            _configurator.SagaConfigured(configurator);
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
        {
            _configurator.SagaMessageConfigured(configurator);
        }

        ConnectHandle IHandlerConfigurationObserverConnector.ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _configurator.ConnectHandlerConfigurationObserver(observer);
        }

        void IHandlerConfigurationObserver.HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
        {
            _configurator.HandlerConfigured(configurator);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configurator.ConnectReceiveEndpointObserver(observer);
        }
    }
}
