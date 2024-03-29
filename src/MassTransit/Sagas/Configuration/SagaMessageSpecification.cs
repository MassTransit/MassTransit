﻿namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public partial class SagaConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Configures the pipe for a Saga/message combination within a Saga configuration
        /// block. Does not add any handlers to the message pipe standalone, everything is within
        /// the Saga pipe segment.
        /// </summary>
        public class SagaMessageSpecification :
            ISagaMessageSpecification<TSaga, TMessage>
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

                throw new ArgumentException($"The message type was invalid: {TypeCache<T>.ShortName}");
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
                _observers.ForEach(observer => observer.SagaMessageConfigured(this));

                if (_configurator == null)
                    throw new ArgumentNullException(nameof(_configurator));

                _configurator.AddPipeSpecification(new FilterPipeSpecification<SagaConsumeContext<TSaga, TMessage>>(consumeFilter));

                return _configurator.Build();
            }

            public IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure)
            {
                configure?.Invoke(_messagePipeConfigurator);

                return _messagePipeConfigurator.Build();
            }

            public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
            {
                _configurator.AddPipeSpecification(new SagaPipeSpecificationProxy(specification));
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
                    _configurator.AddPipeSpecification(new SagaPipeSpecificationProxy(specification));
                }
            }
        }
    }
}
