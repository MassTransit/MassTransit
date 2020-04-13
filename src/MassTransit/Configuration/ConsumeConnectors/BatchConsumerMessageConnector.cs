namespace MassTransit.ConsumeConnectors
{
    using System;
    using ConsumerSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.ConsumerFactories;
    using Pipeline.Filters;


    public class BatchConsumerMessageConnector<TConsumer, TMessage> :
        IConsumerMessageConnector<TConsumer>
        where TConsumer : class, IConsumer<Batch<TMessage>>
        where TMessage : class
    {
        public Type MessageType => typeof(TMessage);

        public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
        {
            return new BatchConsumerMessageSpecification<TConsumer, TMessage>();
        }

        public ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification)
        {
            var options = specification.Options<BatchOptions>();

            int messageLimit = options.MessageLimit;
            TimeSpan timeLimit = options.TimeLimit;

            var batchMessageSpecification = specification.GetMessageSpecification<Batch<TMessage>>();

            var consumeFilter = new MethodConsumerMessageFilter<TConsumer, Batch<TMessage>>();

            var batchConsumerPipe = batchMessageSpecification.Build(consumeFilter);

            var batchConsumerFactory = new BatchConsumerFactory<TConsumer, TMessage>(consumerFactory, messageLimit, timeLimit, batchConsumerPipe);

            var messageConsumerSpecification = ConsumerConnectorCache<IConsumer<TMessage>>.Connector.CreateConsumerSpecification<IConsumer<TMessage>>();

            var messageSpecification = messageConsumerSpecification.GetMessageSpecification<TMessage>();

            var consumerPipe = messageSpecification.Build(new MethodConsumerMessageFilter<IConsumer<TMessage>, TMessage>());

            IPipe<ConsumeContext<TMessage>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                x.UseFilter(new ConsumerMessageFilter<IConsumer<TMessage>, TMessage>(batchConsumerFactory, consumerPipe));
            });

            return consumePipe.ConnectConsumePipe(messagePipe);
        }
    }
}
