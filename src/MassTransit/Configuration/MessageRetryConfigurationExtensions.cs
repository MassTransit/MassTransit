namespace MassTransit
{
    using System;
    using System.Threading;
    using Configuration;
    using Middleware;


    public static class MessageRetryConfigurationExtensions
    {
        /// <summary>
        /// For all configured messages type (handlers, consumers, and sagas), configures message retry using the retry configuration specified.
        /// Retry is configured once for each message type, and is added prior to the consumer factory or saga repository in the pipeline.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseMessageRetry(this IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var observer = new MessageRetryConfigurationObserver(configurator, CancellationToken.None, configure);
        }

        /// <summary>
        /// For all configured messages type (handlers, consumers, and sagas), configures message retry using the retry configuration specified.
        /// Retry is configured once for each message type, and is added prior to the consumer factory or saga repository in the pipeline.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connector">
        /// The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped
        /// </param>
        /// <param name="configure"></param>
        public static void UseMessageRetry(this IConsumePipeConfigurator configurator, IBusFactoryConfigurator connector,
            Action<IRetryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var retryObserver = new RetryBusObserver();
            connector.ConnectBusObserver(retryObserver);

            var observer = new MessageRetryConfigurationObserver(configurator, retryObserver.Stopping, configure);
        }

        /// <summary>
        /// Configures the message retry for the consumer consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseMessageRetry<TConsumer>(this IConsumerConfigurator<TConsumer> configurator, Action<IRetryConfigurator> configure)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new MessageRetryConsumerConfigurationObserver<TConsumer>(configurator, CancellationToken.None, configure);
            configurator.ConnectConsumerConfigurationObserver(observer);
        }

        /// <summary>
        /// Configures the message retry for the consumer consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="busFactoryConfigurator">
        /// The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped
        /// </param>
        /// <param name="configure"></param>
        public static void UseMessageRetry<TConsumer>(this IConsumerConfigurator<TConsumer> configurator, IBusFactoryConfigurator busFactoryConfigurator,
            Action<IRetryConfigurator> configure)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var retryObserver = new RetryBusObserver();
            busFactoryConfigurator.ConnectBusObserver(retryObserver);

            var observer = new MessageRetryConsumerConfigurationObserver<TConsumer>(configurator, retryObserver.Stopping, configure);
            configurator.ConnectConsumerConfigurationObserver(observer);
        }

        /// <summary>
        /// Configures the message retry for the consumer consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseMessageRetry<TSaga>(this ISagaConfigurator<TSaga> configurator, Action<IRetryConfigurator> configure)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new MessageRetrySagaConfigurationObserver<TSaga>(configurator, CancellationToken.None, configure);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Configures the message retry for the consumer consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="busFactoryConfigurator">
        /// The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped
        /// </param>
        /// <param name="configure"></param>
        public static void UseMessageRetry<TSaga>(this ISagaConfigurator<TSaga> configurator, IBusFactoryConfigurator busFactoryConfigurator,
            Action<IRetryConfigurator> configure)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var retryObserver = new RetryBusObserver();
            busFactoryConfigurator.ConnectBusObserver(retryObserver);

            var observer = new MessageRetrySagaConfigurationObserver<TSaga>(configurator, retryObserver.Stopping, configure);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Configures the message retry for the consumer consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseMessageRetry<TMessage>(this IHandlerConfigurator<TMessage> configurator, Action<IRetryConfigurator> configure)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new MessageRetryHandlerConfigurationObserver(CancellationToken.None, configure);
            configurator.ConnectHandlerConfigurationObserver(observer);
        }

        /// <summary>
        /// Configures the message retry for the consumer consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="busFactoryConfigurator">
        /// The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped
        /// </param>
        /// <param name="configure"></param>
        public static void UseMessageRetry<TMessage>(this IHandlerConfigurator<TMessage> configurator, IBusFactoryConfigurator busFactoryConfigurator,
            Action<IRetryConfigurator> configure)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var retryObserver = new RetryBusObserver();
            busFactoryConfigurator.ConnectBusObserver(retryObserver);

            var observer = new MessageRetryHandlerConfigurationObserver(retryObserver.Stopping, configure);
            configurator.ConnectHandlerConfigurationObserver(observer);
        }
    }
}
