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
namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configuration;
    using ConsumeConfigurators;
    using EndpointSpecifications;
    using GreenPipes;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using Topology.Configuration;


    public abstract class BusFactoryConfigurator<TBuilder>
        where TBuilder : IBusBuilder
    {
        readonly IEndpointConfiguration _configuration;
        readonly IList<IReceiveEndpointSpecification<TBuilder>> _endpointSpecifications;
        readonly IList<IBusFactorySpecification<TBuilder>> _specifications;

        protected BusFactoryConfigurator(IEndpointConfiguration configuration)
        {
            _configuration = configuration;

            _specifications = new List<IBusFactorySpecification<TBuilder>>();
            _endpointSpecifications = new List<IReceiveEndpointSpecification<TBuilder>>();
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

        public void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessageSendTopologyConfigurator<T> configurator = _configuration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            var configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public ISendTopologyConfigurator SendTopology => _configuration.Topology.Send;

        public IPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public void AddBusFactorySpecification(IBusFactorySpecification specification)
        {
            _specifications.Add(CreateSpecificationProxy(specification));
        }

        public void AddBusFactorySpecification(IBusFactorySpecification<TBuilder> specification)
        {
            _specifications.Add(specification);
        }

        public void AddReceiveEndpointSpecification(IReceiveEndpointSpecification<TBuilder> specification)
        {
            _endpointSpecifications.Add(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configuration.Consume.Configurator.ConnectConsumerConfigurationObserver(observer);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configuration.Consume.Configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Before configuring any topology options, calling this will make it so that send and publish
        /// topologies are completely separated for this bus. This means that some types may not properly
        /// follow the topology rules, so use with caution.
        /// </summary>
        public void SeparatePublishFromSendTopology()
        {
            _configuration.Topology.SeparatePublishFromSendTopology();
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate())
                .Concat(_endpointSpecifications.SelectMany(x => x.Validate()))
                .Concat(_configuration.Validate());
        }

        protected void ApplySpecifications(TBuilder builder)
        {
            foreach (IBusFactorySpecification<TBuilder> configurator in _specifications)
                configurator.Apply(builder);

            foreach (IReceiveEndpointSpecification<TBuilder> configurator in _endpointSpecifications)
                configurator.Apply(builder);
        }

        protected virtual IBusFactorySpecification<TBuilder> CreateSpecificationProxy(IBusFactorySpecification specification)
        {
            return new ConfiguratorProxy(specification);
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator) where TConsumer : class
        {
            _configuration.Consume.Configurator.ConsumerConfigured(configurator);
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator) where TConsumer : class
            where TMessage : class
        {
            _configuration.Consume.Configurator.ConsumerMessageConfigured(configurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator) where TSaga : class, ISaga
        {
            _configuration.Consume.Configurator.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator) where TSaga : class, ISaga
            where TMessage : class
        {
            _configuration.Consume.Configurator.SagaMessageConfigured(configurator);
        }


        class ConfiguratorProxy :
            IBusFactorySpecification<TBuilder>
        {
            readonly IBusFactorySpecification _configurator;

            public ConfiguratorProxy(IBusFactorySpecification configurator)
            {
                _configurator = configurator;
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _configurator.Validate();
            }

            public void Apply(TBuilder builder)
            {
                _configurator.Apply(builder);
            }
        }
    }
}