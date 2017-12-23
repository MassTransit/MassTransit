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
namespace MassTransit.ConsumePipeSpecifications
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Filters;
    using Metadata;
    using Observables;
    using Pipeline;
    using Pipeline.Pipes;
    using Saga;
    using Saga.SubscriptionConfigurators;


    public class ConsumePipeSpecification :
        IConsumePipeConfigurator,
        IConsumePipeSpecification
    {
        readonly ConsumerConfigurationObservable _consumerObservers;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessageConsumePipeSpecification> _messageSpecifications;
        readonly ConsumePipeSpecificationObservable _observers;
        readonly SagaConfigurationObservable _sagaObservers;
        readonly IList<IPipeSpecification<ConsumeContext>> _specifications;
        readonly IList<IPipeSpecification<ConsumeContext>> _consumeContextSpecifications;

        public ConsumePipeSpecification()
        {
            _specifications = new List<IPipeSpecification<ConsumeContext>>();
            _consumeContextSpecifications = new List<IPipeSpecification<ConsumeContext>>();
            _messageSpecifications = new ConcurrentDictionary<Type, IMessageConsumePipeSpecification>();
            _observers = new ConsumePipeSpecificationObservable();
            _consumerObservers = new ConsumerConfigurationObservable();
            _sagaObservers = new SagaConfigurationObservable();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            lock (_lock)
            {
                _specifications.Add(specification);

                foreach (var messageSpecification in _messageSpecifications.Values)
                {
                    messageSpecification.AddPipeSpecification(specification);
                }
            }
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _consumerObservers.Connect(observer);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _sagaObservers.Connect(observer);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            IMessageConsumePipeSpecification<T> messageSpecification = GetMessageSpecification<T>();

            messageSpecification.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumeContextSpecifications.Add(specification);
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            _consumerObservers.ConsumerConfigured(configurator);
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            _consumerObservers.ConsumerMessageConfigured(configurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator) where TSaga : class, ISaga
        {
            _sagaObservers.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _sagaObservers.SagaMessageConfigured(configurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate())
                .Concat(_messageSpecifications.Values.SelectMany(x => x.Validate()));
        }

        public IMessageConsumePipeSpecification<T> GetMessageSpecification<T>()
            where T : class
        {
            var specification = _messageSpecifications.GetOrAdd(typeof(T), CreateMessageSpecification<T>);

            return specification.GetMessageSpecification<T>();
        }

        public ConnectHandle Connect(IConsumePipeSpecificationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public IConsumePipe BuildConsumePipe()
        {
            var filter = new DynamicFilter<ConsumeContext, Guid>(new ConsumeContextConverterFactory(), GetRequestId);

            foreach (var specification in _messageSpecifications.Values)
            {
                specification.Connect(filter);
            }

            var builder = new PipeBuilder<ConsumeContext>();
            foreach (var specification in _consumeContextSpecifications)
            {
                specification.Apply(builder);
            }
            builder.AddFilter(filter);

            return new ConsumePipe(filter, builder.Build());
        }

        static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }

        IMessageConsumePipeSpecification CreateMessageSpecification<T>(Type type)
            where T : class
        {
            var specification = new MessageConsumePipeSpecification<T>();

            foreach (var pipeSpecification in _specifications)
            {
                specification.AddPipeSpecification(pipeSpecification);
            }

            _observers.MessageSpecificationCreated(specification);

            var connector = new ImplementedMessageTypeConnector<T>(this, specification);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            return specification;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly MessageConsumePipeSpecification<TMessage> _messageSpecification;
            readonly IConsumePipeSpecification _specification;

            public ImplementedMessageTypeConnector(IConsumePipeSpecification specification, MessageConsumePipeSpecification<TMessage> messageSpecification)
            {
                _specification = specification;
                _messageSpecification = messageSpecification;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IMessageConsumePipeSpecification<T> implementedTypeSpecification = _specification.GetMessageSpecification<T>();

                _messageSpecification.AddImplementedMessageSpecification(implementedTypeSpecification);
            }
        }
    }
}