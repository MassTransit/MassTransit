namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Observables;


    /// <summary>
    /// Configures the pipe for a consumer/message combination within a consumer configuration
    /// block. Does not add any handlers to the message pipe standalone, everything is within
    /// the consumer pipe segment.
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class BatchConsumerMessageSpecification<TConsumer, TMessage> :
        IConsumerMessageSpecification<TConsumer, Batch<TMessage>>,
        IConsumerMessageConfigurator<TConsumer, TMessage>
        where TMessage : class
        where TConsumer : class, IConsumer<Batch<TMessage>>
    {
        readonly IBuildPipeConfigurator<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> _batchConfigurator;
        readonly IBuildPipeConfigurator<ConsumeContext<Batch<TMessage>>> _batchMessagePipeConfigurator;
        readonly ConsumerMessageSpecification<TConsumer, TMessage> _consumerSpecification;
        readonly ConsumerConfigurationObservable _observers;

        public BatchConsumerMessageSpecification()
        {
            _batchConfigurator = new PipeConfigurator<ConsumerConsumeContext<TConsumer, Batch<TMessage>>>();
            _batchMessagePipeConfigurator = new PipeConfigurator<ConsumeContext<Batch<TMessage>>>();

            _consumerSpecification = new ConsumerMessageSpecification<TConsumer, TMessage>();
            _observers = new ConsumerConfigurationObservable();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>> specification)
        {
            _consumerSpecification.AddPipeSpecification(specification);
        }

        public void Message(Action<IConsumerMessageConfigurator<TMessage>> configure)
        {
            _consumerSpecification.Message(configure);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            var batchSpecification = this as IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>;
            _observers.ForEach(observer => observer.ConsumerMessageConfigured(batchSpecification));

            return _batchConfigurator.Validate()
                .Concat(_batchMessagePipeConfigurator.Validate())
                .Concat(_consumerSpecification.Validate());
        }

        public Type MessageType => typeof(TMessage);

        public bool TryGetMessageSpecification<TC, T>(out IConsumerMessageSpecification<TC, T> specification)
            where T : class
            where TC : class
        {
            specification = this as IConsumerMessageSpecification<TC, T>;
            return specification != null
                || _consumerSpecification.TryGetMessageSpecification(out specification);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> specification)
        {
            _batchConfigurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<Batch<TMessage>>> specification)
        {
            _batchMessagePipeConfigurator.AddPipeSpecification(specification);
        }

        public IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> Build(IFilter<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumeFilter)
        {
            _batchConfigurator.UseFilter(consumeFilter);

            return _batchConfigurator.Build();
        }

        public IPipe<ConsumeContext<Batch<TMessage>>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<Batch<TMessage>>>> configure)
        {
            configure?.Invoke(_batchMessagePipeConfigurator);

            return _batchMessagePipeConfigurator.Build();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            _batchConfigurator.AddPipeSpecification(new ConsumerPipeSpecificationProxy<TConsumer, Batch<TMessage>>(specification));
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public void Message(Action<IConsumerMessageConfigurator<Batch<TMessage>>> configure)
        {
            configure?.Invoke(new ConsumerMessageConfigurator(_batchMessagePipeConfigurator));
        }


        class ConsumerMessageConfigurator :
            IConsumerMessageConfigurator<Batch<TMessage>>
        {
            readonly IBuildPipeConfigurator<ConsumeContext<Batch<TMessage>>> _batchConfigurator;

            public ConsumerMessageConfigurator(IBuildPipeConfigurator<ConsumeContext<Batch<TMessage>>> batchConfigurator)
            {
                _batchConfigurator = batchConfigurator;
            }

            public void AddPipeSpecification(IPipeSpecification<ConsumeContext<Batch<TMessage>>> specification)
            {
                _batchConfigurator.AddPipeSpecification(specification);
            }
        }
    }
}
