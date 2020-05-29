namespace MassTransit.ConsumeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;
    using Pipeline.ConsumerFactories;
    using Pipeline.Filters;


    public class BatchConsumerSpecification<TConsumer, TMessage> :
        IReceiveEndpointSpecification
        where TMessage : class
        where TConsumer : class, IConsumer<Batch<TMessage>>
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly int _messageLimit;
        readonly IConsumerMessageSpecification<TConsumer, Batch<TMessage>> _messageSpecification;
        readonly TimeSpan _timeLimit;

        public BatchConsumerSpecification(IConsumerMessageSpecification<TConsumer, Batch<TMessage>> messageSpecification,
            IConsumerFactory<TConsumer> consumerFactory, int messageLimit, TimeSpan timeLimit)
        {
            _messageSpecification = messageSpecification;
            _consumerFactory = consumerFactory;
            _messageLimit = messageLimit;
            _timeLimit = timeLimit;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerFactory.Validate().Concat(_messageSpecification.Validate());
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            var filter = new MethodConsumerMessageFilter<TConsumer, Batch<TMessage>>();

            IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumerPipe = _messageSpecification.Build(filter);

            var batchConsumerFactory = new BatchConsumerFactory<TConsumer, TMessage>(_consumerFactory, _messageLimit, _timeLimit, consumerPipe);

            IConsumerSpecification<IConsumer<TMessage>> specification =
                ConsumerConnectorCache<IConsumer<TMessage>>.Connector.CreateConsumerSpecification<IConsumer<TMessage>>();

            ConsumerConnectorCache<IConsumer<TMessage>>.Connector.ConnectConsumer(builder, batchConsumerFactory, specification);
        }
    }
}
