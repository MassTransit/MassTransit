namespace MassTransit
{
    using System;
    using System.Threading;
    using Configuration;
    using RetryPolicies;


    public static class ScheduledRedeliveryConfigurationExtensions
    {
        /// <summary>
        /// Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy, via
        /// the delayed exchange feature of ActiveMQ.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseScheduledRedelivery<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRetryConfigurator> configure)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var redeliverySpecification = new ScheduledRedeliveryPipeSpecification<T>();

            configurator.AddPipeSpecification(redeliverySpecification);

            var retrySpecification = new RedeliveryRetryPipeSpecification<T>(redeliverySpecification);

            configure?.Invoke(retrySpecification);

            configurator.AddPipeSpecification(retrySpecification);
        }

        /// <summary>
        /// Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryPolicy"></param>
        public static void UseScheduledRedelivery<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IRetryPolicy retryPolicy)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var redeliverySpecification = new ScheduledRedeliveryPipeSpecification<T>();

            configurator.AddPipeSpecification(redeliverySpecification);

            var retrySpecification = new RedeliveryRetryPipeSpecification<T>(redeliverySpecification);

            retrySpecification.SetRetryPolicy(exceptionFilter =>
                new ConsumeContextRetryPolicy<ConsumeContext<T>, RetryConsumeContext<T>>(retryPolicy, CancellationToken.None, Factory));

            configurator.AddPipeSpecification(retrySpecification);
        }

        static RetryConsumeContext<T> Factory<T>(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where T : class
        {
            return new RetryConsumeContext<T>(context, retryPolicy, retryContext);
        }

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

            var observer = new ScheduledRedeliveryConsumerConfigurationObserver<TConsumer>(configurator, configure);
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

            var observer = new ScheduledRedeliverySagaConfigurationObserver<TSaga>(configurator, configure);
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

            var observer = new ScheduledRedeliveryHandlerConfigurationObserver(configure);
            configurator.ConnectHandlerConfigurationObserver(observer);
        }
    }
}
