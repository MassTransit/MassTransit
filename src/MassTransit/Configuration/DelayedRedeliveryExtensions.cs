namespace MassTransit
{
    using System;
    using System.Threading;
    using Configuration;
    using RetryPolicies;


    public static class DelayedRedeliveryExtensions
    {
        /// <summary>
        /// Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy, via
        /// the delayed exchange feature of ActiveMQ.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseDelayedRedelivery<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRedeliveryConfigurator> configure)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var redeliverySpecification = new DelayedRedeliveryPipeSpecification<T>();
            var retrySpecification = new RedeliveryRetryPipeSpecification<T>(redeliverySpecification);
            configure?.Invoke(retrySpecification);

            configurator.AddPipeSpecification(redeliverySpecification);
            configurator.AddPipeSpecification(retrySpecification);
        }

        /// <summary>
        /// Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryPolicy"></param>
        public static void UseDelayedRedelivery<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IRetryPolicy retryPolicy)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var redeliverySpecification = new DelayedRedeliveryPipeSpecification<T>();
            var retrySpecification = new RedeliveryRetryPipeSpecification<T>(redeliverySpecification);

            retrySpecification.SetRetryPolicy(exceptionFilter =>
                new ConsumeContextRetryPolicy<ConsumeContext<T>, RetryConsumeContext<T>>(retryPolicy, CancellationToken.None, Factory));

            configurator.AddPipeSpecification(redeliverySpecification);
            configurator.AddPipeSpecification(retrySpecification);
        }

        static RetryConsumeContext<T> Factory<T>(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where T : class
        {
            return new RetryConsumeContext<T>(context, retryPolicy, retryContext);
        }

        /// <summary>
        /// For all configured messages type (handlers, consumers, and sagas), configures delayed redelivery using the retry configuration specified.
        /// Redelivery is configured once for each message type, and is added prior to the consumer factory or saga repository in the pipeline.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configureRetry"></param>
        public static void UseDelayedRedelivery(this IConsumePipeConfigurator configurator, Action<IRedeliveryConfigurator> configureRetry)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (configureRetry == null)
                throw new ArgumentNullException(nameof(configureRetry));

            var observer = new DelayedRedeliveryConfigurationObserver(configurator, configureRetry);
        }

        /// <summary>
        /// Configure scheduled redelivery for the consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseDelayedRedelivery<TConsumer>(this IConsumerConfigurator<TConsumer> configurator, Action<IRedeliveryConfigurator> configure)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new DelayedRedeliveryConsumerConfigurationObserver<TConsumer>(configurator, configure);
            configurator.ConnectConsumerConfigurationObserver(observer);
        }

        /// <summary>
        /// Configure scheduled redelivery for the saga, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseDelayedRedelivery<TSaga>(this ISagaConfigurator<TSaga> configurator, Action<IRedeliveryConfigurator> configure)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new DelayedRedeliverySagaConfigurationObserver<TSaga>(configurator, configure);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Configures the message retry for the handler, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseDelayedRedelivery<TMessage>(this IHandlerConfigurator<TMessage> configurator, Action<IRedeliveryConfigurator> configure)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new DelayedRedeliveryHandlerConfigurationObserver(configure);
            configurator.ConnectHandlerConfigurationObserver(observer);
        }
    }
}
