namespace MassTransit.Configuration
{
    using System;


    public interface ISagaMessageSpecification<TSaga> :
        IPipeConfigurator<SagaConsumeContext<TSaga>>,
        ISagaConfigurationObserverConnector,
        ISpecification
        where TSaga : class, ISaga
    {
        Type MessageType { get; }

        ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>()
            where T : class;
    }


    public interface ISagaMessageSpecification<TSaga, TMessage> :
        ISagaMessageSpecification<TSaga>,
        ISagaMessageConfigurator<TSaga, TMessage>,
        ISagaMessageConfigurator<TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Build the consumer pipe, using the consume filter specified.
        /// </summary>
        /// <param name="consumeFilter"></param>
        /// <returns></returns>
        IPipe<SagaConsumeContext<TSaga, TMessage>> BuildConsumerPipe(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter);

        /// <summary>
        /// Configure the message pipe as it is built. Any previously configured filters will precede
        /// the configuration applied by the <paramref name="configure" /> callback.
        /// </summary>
        /// <param name="configure">Configure the message pipe</param>
        /// <returns></returns>
        IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure);
    }
}
