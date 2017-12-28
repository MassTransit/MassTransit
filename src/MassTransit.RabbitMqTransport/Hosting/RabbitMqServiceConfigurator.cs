// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Hosting
{
    using System;
    using Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Hosting;
    using MassTransit.Topology.Configuration;
    using Saga;
    using Saga.SubscriptionConfigurators;


    /// <summary>
    /// A hosted service can specify receive endpoints using the service configurator
    /// </summary>
    public class RabbitMqServiceConfigurator :
        IServiceConfigurator
    {
        readonly IRabbitMqBusFactoryConfigurator _configurator;
        readonly int _defaultConsumerLimit;
        readonly IRabbitMqHost _host;

        public RabbitMqServiceConfigurator(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            _configurator = configurator;
            _host = host;
            _defaultConsumerLimit = Environment.ProcessorCount * 4;
        }

        public void ReceiveEndpoint(string queueName, int consumerLimit, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator.ReceiveEndpoint(_host, queueName, x =>
            {
                x.PrefetchCount = (ushort)consumerLimit;

                configureEndpoint(x);
            });
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPrePipeSpecification(specification);
        }

        public ISendTopologyConfigurator SendTopology => _configurator.SendTopology;

        public IPublishTopologyConfigurator PublishTopology => _configurator.PublishTopology;

        public void AddBusFactorySpecification(IBusFactorySpecification specification)
        {
            _configurator.AddBusFactorySpecification(specification);
        }

        public void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology) where T : class
        {
            ((IBusFactoryConfigurator)_configurator).Send(configureTopology);
        }

        public void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology) where T : class
        {
            ((IBusFactoryConfigurator)_configurator).Publish(configureTopology);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, _defaultConsumerLimit, configureEndpoint);
        }

        ConnectHandle IConsumerConfigurationObserverConnector.ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configurator.ConnectConsumerConfigurationObserver(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
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

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configurator.ConnectSagaConfigurationObserver(observer);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator) where TSaga : class, ISaga
        {
            _configurator.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator) where TSaga : class, ISaga where TMessage : class
        {
            _configurator.SagaMessageConfigured(configurator);
        }
    }
}