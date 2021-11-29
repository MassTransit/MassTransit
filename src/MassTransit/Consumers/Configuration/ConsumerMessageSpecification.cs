namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


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
            _observers.ForEach(observer => observer.ConsumerMessageConfigured(this));

            return _configurator.Validate()
                .Concat(_messagePipeConfigurator.Validate());
        }

        public Type MessageType => typeof(TMessage);

        public bool TryGetMessageSpecification<TC, T>(out IConsumerMessageSpecification<TC, T> specification)
            where T : class
            where TC : class
        {
            specification = this as IConsumerMessageSpecification<TC, T>;
            return specification != null;
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
