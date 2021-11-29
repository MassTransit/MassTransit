namespace MassTransit
{
    using System;


    public interface IConsumerMessageConfigurator<TMessage> :
        IPipeConfigurator<ConsumeContext<TMessage>>
        where TMessage : class
    {
    }


    public interface IConsumerMessageConfigurator<TConsumer, TMessage> :
        IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        /// <summary>
        /// Add middleware to the consumer pipeline, for the specified message type, which is
        /// invoked after the consumer factory.
        /// </summary>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void Message(Action<IConsumerMessageConfigurator<TMessage>> configure);
    }
}
