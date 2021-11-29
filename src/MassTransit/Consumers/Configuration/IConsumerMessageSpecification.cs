namespace MassTransit.Configuration
{
    using System;


    public interface IConsumerMessageSpecification<TConsumer> :
        IPipeConfigurator<ConsumerConsumeContext<TConsumer>>,
        IConsumerConfigurationObserverConnector,
        ISpecification
        where TConsumer : class
    {
        Type MessageType { get; }

        bool TryGetMessageSpecification<TC, T>(out IConsumerMessageSpecification<TC, T> specification)
            where T : class
            where TC : class;
    }


    public interface IConsumerMessageSpecification<TConsumer, TMessage> :
        IConsumerMessageSpecification<TConsumer>,
        IConsumerMessageConfigurator<TConsumer, TMessage>,
        IConsumerMessageConfigurator<TMessage>
        where TConsumer : class
        where TMessage : class
    {
        IPipe<ConsumerConsumeContext<TConsumer, TMessage>> Build(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter);

        /// <summary>
        /// Configure the message pipe as it is built. Any previously configured filters will precede
        /// the configuration applied by the <paramref name="configure" /> callback.
        /// </summary>
        /// <param name="configure">Configure the message pipe</param>
        /// <returns></returns>
        IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure);
    }
}
