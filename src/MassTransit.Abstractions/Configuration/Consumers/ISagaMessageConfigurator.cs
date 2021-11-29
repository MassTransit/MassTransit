namespace MassTransit
{
    using System;


    public interface ISagaMessageConfigurator<TMessage> :
        IPipeConfigurator<ConsumeContext<TMessage>>
        where TMessage : class
    {
    }


    public interface ISagaMessageConfigurator<TSaga, TMessage> :
        IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Add middleware to the saga pipeline, for the specified message type, which is
        /// invoked after the saga repository.
        /// </summary>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void Message(Action<ISagaMessageConfigurator<TMessage>> configure);
    }
}
