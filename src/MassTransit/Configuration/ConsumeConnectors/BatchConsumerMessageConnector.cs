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

            var messageLimit = options.MessageLimit;
            var timeLimit = options.TimeLimit;

            IConsumerMessageSpecification<TConsumer, Batch<TMessage>> batchMessageSpecification = specification.GetMessageSpecification<Batch<TMessage>>();

            var consumeFilter = new MethodConsumerMessageFilter<TConsumer, Batch<TMessage>>();

            IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> batchConsumerPipe = batchMessageSpecification.Build(consumeFilter);

            var batchConsumerFactory = new BatchConsumerFactory<TConsumer, TMessage>(consumerFactory, messageLimit, timeLimit, batchConsumerPipe);

            IConsumerSpecification<IConsumer<TMessage>> messageConsumerSpecification =
                ConsumerConnectorCache<IConsumer<TMessage>>.Connector.CreateConsumerSpecification<IConsumer<TMessage>>();

            IConsumerMessageSpecification<IConsumer<TMessage>, TMessage> messageSpecification =
                messageConsumerSpecification.GetMessageSpecification<TMessage>();

            IPipe<ConsumerConsumeContext<IConsumer<TMessage>, TMessage>> consumerPipe =
                messageSpecification.Build(new MethodConsumerMessageFilter<IConsumer<TMessage>, TMessage>());

            IPipe<ConsumeContext<TMessage>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                x.UseFilter(new ConsumerMessageFilter<IConsumer<TMessage>, TMessage>(batchConsumerFactory, consumerPipe));
            });

            return consumePipe.ConnectConsumePipe(messagePipe);
        }
    }
}
