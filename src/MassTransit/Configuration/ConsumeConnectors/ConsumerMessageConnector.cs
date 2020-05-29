namespace MassTransit.ConsumeConnectors
{
    using System;
    using ConsumerSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Filters;


    public class ConsumerMessageConnector<TConsumer, TMessage> :
        IConsumerMessageConnector<TConsumer>
        where TConsumer : class
        where TMessage : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _consumeFilter;

        public ConsumerMessageConnector(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
        {
            _consumeFilter = consumeFilter;
        }

        public Type MessageType => typeof(TMessage);

        public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
        {
            return new ConsumerMessageSpecification<TConsumer, TMessage>();
        }

        public ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification)
        {
            IConsumerMessageSpecification<TConsumer, TMessage> messageSpecification = specification.GetMessageSpecification<TMessage>();

            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> consumerPipe = messageSpecification.Build(_consumeFilter);

            IPipe<ConsumeContext<TMessage>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                x.UseFilter(new ConsumerMessageFilter<TConsumer, TMessage>(consumerFactory, consumerPipe));
            });

            return consumePipe.ConnectConsumePipe(messagePipe);
        }
    }
}
