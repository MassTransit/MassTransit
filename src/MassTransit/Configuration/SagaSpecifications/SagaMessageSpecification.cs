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
namespace MassTransit.SagaSpecifications
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Metadata;
    using Saga;
    using SagaConfigurators;
    using Util;


    /// <summary>
    /// Configures the pipe for a Saga/message combination within a Saga configuration
    /// block. Does not add any handlers to the message pipe standalone, everything is within
    /// the Saga pipe segment.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaMessageSpecification<TSaga, TMessage> :
        ISagaMessageSpecification<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IBuildPipeConfigurator<SagaConsumeContext<TSaga, TMessage>> _configurator;
        readonly IBuildPipeConfigurator<ConsumeContext<TMessage>> _messagePipeConfigurator;
        readonly SagaConfigurationObservable _observers;

        public SagaMessageSpecification()
        {
            _configurator = new PipeConfigurator<SagaConsumeContext<TSaga, TMessage>>();
            _messagePipeConfigurator = new PipeConfigurator<ConsumeContext<TMessage>>();
            _observers = new SagaConfigurationObservable();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate();
        }

        public Type MessageType => typeof(TMessage);

        ISagaMessageSpecification<TSaga, T> ISagaMessageSpecification<TSaga>.GetMessageSpecification<T>()
        {
            if (this is ISagaMessageSpecification<TSaga, T> result)
                return result;

            throw new ArgumentException($"The message type was invalid: {TypeMetadataCache<T>.ShortName}");
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga, TMessage>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _messagePipeConfigurator.AddPipeSpecification(specification);
        }

        public IPipe<SagaConsumeContext<TSaga, TMessage>> BuildConsumerPipe(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter)
        {
            _observers.All(observer =>
            {
                observer.SagaMessageConfigured(this);
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

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            _configurator.AddPipeSpecification(new SagaPipeSpecificationProxy<TSaga, TMessage>(specification));
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public void Message(Action<ISagaMessageConfigurator<TMessage>> configure)
        {
            configure?.Invoke(new SagaMessageConfigurator(_configurator));
        }


        class SagaMessageConfigurator :
            ISagaMessageConfigurator<TMessage>
        {
            readonly IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>> _configurator;

            public SagaMessageConfigurator(IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>> configurator)
            {
                _configurator = configurator;
            }

            public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
            {
                _configurator.AddPipeSpecification(new SagaPipeSpecificationProxy<TSaga, TMessage>(specification));
            }
        }
    }
}
