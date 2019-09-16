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
namespace MassTransit.ConsumerSpecifications
{
    using System;
    using System.Collections.Generic;
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Metadata;
    using Util;


    /// <summary>
    /// Configures the pipe for a consumer/message combination within a consumer configuration
    /// block. Does not add any handlers to the message pipe standalone, everything is within
    /// the consumer pipe segment.
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerMessageSpecification<TConsumer, TMessage> :
        IConsumerMessageSpecification<TConsumer, TMessage>
        where TMessage : class
        where TConsumer : class
    {
        readonly IBuildPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>> _configurator;
        readonly IBuildPipeConfigurator<ConsumeContext<TMessage>> _messagePipeConfigurator;
        readonly ConsumerConfigurationObservable _observers;

        public ConsumerMessageSpecification()
        {
            _configurator = new PipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>>();
            _messagePipeConfigurator = new PipeConfigurator<ConsumeContext<TMessage>>();
            _observers = new ConsumerConfigurationObservable();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate();
        }

        public Type MessageType => typeof(TMessage);

        IConsumerMessageSpecification<TConsumer, T> IConsumerMessageSpecification<TConsumer>.GetMessageSpecification<T>()
        {
            var result = this as IConsumerMessageSpecification<TConsumer, T>;
            if (result == null)
                throw new ArgumentException($"The message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _messagePipeConfigurator.AddPipeSpecification(specification);
        }

        public IPipe<ConsumerConsumeContext<TConsumer, TMessage>> Build(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
        {
            _observers.All(observer =>
            {
                observer.ConsumerMessageConfigured(this);
                return true;
            });

            _configurator.UseFilter(consumeFilter);

            return _configurator.Build();
        }

        public IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure)
        {
            configure?.Invoke(_messagePipeConfigurator);

            return _messagePipeConfigurator.Build();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumerPipeSpecificationProxy<TConsumer, TMessage>(specification));
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public void Message(Action<IConsumerMessageConfigurator<TMessage>> configure)
        {
            configure?.Invoke(new ConsumerMessageConfigurator(_configurator));
        }


        class ConsumerMessageConfigurator :
            IConsumerMessageConfigurator<TMessage>
        {
            readonly IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>> _configurator;

            public ConsumerMessageConfigurator(IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>> configurator)
            {
                _configurator = configurator;
            }

            public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
            {
                _configurator.AddPipeSpecification(new ConsumerPipeSpecificationProxy<TConsumer, TMessage>(specification));
            }
        }
    }
}