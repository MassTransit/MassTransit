namespace MassTransit
{
    using System;
    using Configuration;
    using Consumer;


    public static class BatchConsumerExtensions
    {
        /// <summary>
        /// Configure a Batch&lt;<typeparamref name="TMessage" />&gt; consumer, which allows messages to be collected into an array and consumed
        /// at once. This feature is experimental, but often requested. Be sure to configure the transport with sufficient concurrent message
        /// capacity (prefetch, etc.) so that a batch can actually complete without always reaching the time limit.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Batch<TMessage>(this IReceiveEndpointConfigurator configurator, Action<IBatchConfigurator<TMessage>> configure)
            where TMessage : class
        {
            LogContext.Debug?.Log("Configuring batch: {MessageType}", TypeCache<TMessage>.ShortName);

            var batchConfigurator = new BatchConfigurator<TMessage>(configurator);

            configure?.Invoke(batchConfigurator);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, Func<TConsumer> consumerFactoryMethod)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            LogContext.Debug?.Log("Subscribing Batch Consumer: {ConsumerType} (using delegate consumer factory)", TypeCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactoryMethod);

            configurator.Consumer(delegateConsumerFactory);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactory"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            LogContext.Debug?.Log("Subscribing Batch Consumer: {ConsumerType} (using supplied consumer factory)", TypeCache<TConsumer>.ShortName);

            configurator.Consumer(consumerFactory);
        }
    }
}
