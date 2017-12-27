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
namespace MassTransit.EndpointSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using GreenPipes.Validation;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;
    using Saga;
    using Saga.SubscriptionConfigurators;


    public abstract class ReceiveEndpointSpecification :
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator
    {
        readonly Lazy<Uri> _deadLetterAddress;
        readonly Lazy<Uri> _errorAddress;
        readonly Lazy<Uri> _inputAddress;
        readonly IList<string> _lateConfigurationKeys;
        readonly IBuildPipeConfigurator<ReceiveContext> _receiveConfigurator;
        readonly IList<IReceiveEndpointSpecification> _specifications;
        readonly IEndpointConfiguration _configuration;

        protected ReceiveEndpointSpecification(IEndpointConfiguration configuration)
        {
            _configuration = configuration;

            _specifications = new List<IReceiveEndpointSpecification>();
            _receiveConfigurator = new PipeConfigurator<ReceiveContext>();
            _lateConfigurationKeys = new List<string>();

            _inputAddress = new Lazy<Uri>(GetInputAddress);
            _errorAddress = new Lazy<Uri>(GetErrorAddress);
            _deadLetterAddress = new Lazy<Uri>(GetDeadLetterAddress);
        }

        public Uri InputAddress => _inputAddress.Value;
        public Uri DeadLetterAddress => _deadLetterAddress.Value;
        public Uri ErrorAddress => _errorAddress.Value;

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configuration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            _configuration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configuration.Consume.Configurator.AddPrePipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configuration.Consume.Configurator.ConnectConsumerConfigurationObserver(observer);
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
            _configuration.Consume.Configurator.ConsumerConfigured(configurator);
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            _configuration.Consume.Configurator.ConsumerMessageConfigured(configurator);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configuration.Consume.Configurator.ConnectSagaConfigurationObserver(observer);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            _configuration.Consume.Configurator.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _configuration.Consume.Configurator.SagaMessageConfigured(configurator);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_configuration.Publish.Configurator);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_configuration.Send.Configurator);
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
                .Concat(_configuration.Validate())
                .Concat(_lateConfigurationKeys.Select(
                    x => new ConfigurationValidationResult(ValidationResultDisposition.Failure, x, "was configured after being used")));
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _specifications.Add(configurator);
        }

        protected IReceivePipe CreateReceivePipe(IReceiveEndpointBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Configure(builder);

            _receiveConfigurator.UseDeadLetterQueue(Pipe.New<ReceiveContext>(x => x.UseFilter(new MoveToTransportFilter(DeadLetterAddress, "dead-letter"))));
            _receiveConfigurator.UseRescue(Pipe.New<ExceptionReceiveContext>(x => x.UseFilter(new MoveExceptionToTransportFilter(ErrorAddress))));
            _receiveConfigurator.UseFilter(new DeserializeFilter(builder.MessageDeserializer, builder.ConsumePipe));

            IPipe<ReceiveContext> receivePipe = _receiveConfigurator.Build();

            return new ReceivePipe(receivePipe, builder.ConsumePipe);
        }


        protected virtual void Changed(string key)
        {
            if (IsAlreadyConfigured())
            {
                _lateConfigurationKeys.Add(key);
            }
        }

        protected virtual bool IsAlreadyConfigured()
        {
            return _inputAddress.IsValueCreated
                || _errorAddress.IsValueCreated
                || _deadLetterAddress.IsValueCreated;
        }

        protected abstract Uri GetInputAddress();

        protected abstract Uri GetErrorAddress();

        protected abstract Uri GetDeadLetterAddress();
    }
}