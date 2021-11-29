namespace MassTransit.Configuration
{
    using System;


    public interface IConsumerMessageConnector
    {
        Type MessageType { get; }
    }


    public interface IConsumerMessageConnector<TConsumer> :
        IConsumerMessageConnector
        where TConsumer : class
    {
        IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification();

        ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification);
    }
}
