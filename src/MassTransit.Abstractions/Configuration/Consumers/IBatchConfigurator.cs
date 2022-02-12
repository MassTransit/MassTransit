namespace MassTransit
{
    using System;


    /// <summary>
    /// Batching is an experimental feature, and may be changed at any time in the future.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IBatchConfigurator<TMessage> :
        IConsumeConfigurator
        where TMessage : class
    {
        /// <summary>
        /// Set the maximum time to wait for messages before the batch is automatically completed
        /// </summary>
        TimeSpan TimeLimit { set; }

        /// <summary>
        /// Set the maximum number of messages which can be added to a single batch
        /// </summary>
        int MessageLimit { set; }

        /// <summary>
        /// Set the maximum number of concurrent batches which can execute at the same time
        /// </summary>
        int ConcurrencyLimit { set; }

        /// <summary>
        /// Specify the consumer factory for the batch message consumer
        /// </summary>
        /// <param name="consumerFactory"></param>
        /// <param name="configure">Configure the consumer pipe</param>
        /// <typeparam name="TConsumer"></typeparam>
        void Consumer<TConsumer>(IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>>? configure = null)
            where TConsumer : class, IConsumer<Batch<TMessage>>;
    }
}
