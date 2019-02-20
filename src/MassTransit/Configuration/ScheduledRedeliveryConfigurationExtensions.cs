namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes.Configurators;
    using PipeConfigurators;
    using Saga;


    public static class ScheduledRedeliveryConfigurationExtensions
    {
        /// <summary>
        /// Configure scheduled redelivery for all message types
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configureRetry"></param>
        public static void UseScheduledRedelivery(this IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configureRetry)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (configureRetry == null)
                throw new ArgumentNullException(nameof(configureRetry));

            var observer = new ScheduledRedeliveryConfigurationObserver(configurator, configureRetry);
        }

        /// <summary>
        /// Configure scheduled redelivery for the consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseScheduledRedelivery<TConsumer>(this IConsumerConfigurator<TConsumer> configurator, Action<IRetryConfigurator> configure)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new MessageRedeliveryConsumerConfigurationObserver<TConsumer>(configurator, configure);
            configurator.ConnectConsumerConfigurationObserver(observer);
        }

        /// <summary>
        /// Configure scheduled redelivery for the saga, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseScheduledRedelivery<TSaga>(this ISagaConfigurator<TSaga> configurator, Action<IRetryConfigurator> configure)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new MessageRedeliverySagaConfigurationObserver<TSaga>(configurator, configure);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Configures the message retry for the handler, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseScheduledRedelivery<TMessage>(this IHandlerConfigurator<TMessage> configurator, Action<IRetryConfigurator> configure)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new MessageRedeliveryHandlerConfigurationObserver(configure);
            configurator.ConnectHandlerConfigurationObserver(observer);
        }
    }
}
