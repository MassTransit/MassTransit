namespace MassTransit
{
    using System;
    using System.Linq;
    using Configuration;
    using Middleware;
    using RetryPolicies;


    public static class RetryConfigurationExtensions
    {
        public static void UseRetry(this IPipeConfigurator<ConsumeContext> configurator, Action<IRetryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification();

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRetryConfigurator> configure)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<T>, RetryConsumeContext<T>>(Factory);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry<T>(this IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<T>, RetryConsumeContext<T>>(Factory);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        static RetryConsumeContext<T> Factory<T>(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where T : class
        {
            return new RetryConsumeContext<T>(context, retryPolicy, retryContext);
        }

        public static void UseRetry<TConsumer>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, Action<IRetryConfigurator> configure)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification =
                new ConsumeContextRetryPipeSpecification<ConsumerConsumeContext<TConsumer>, RetryConsumerConsumeContext<TConsumer>>(Factory);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        static RetryConsumerConsumeContext<TConsumer> Factory<TConsumer>(ConsumerConsumeContext<TConsumer> context, IRetryPolicy retryPolicy,
            RetryContext retryContext)
            where TConsumer : class
        {
            return new RetryConsumerConsumeContext<TConsumer>(context, retryPolicy, retryContext);
        }

        public static void UseRetry<TSaga>(this IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, Action<IRetryConfigurator> configure)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification<SagaConsumeContext<TSaga>, RetrySagaConsumeContext<TSaga>>(Factory);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        static RetrySagaConsumeContext<TSaga> Factory<TSaga>(SagaConsumeContext<TSaga> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where TSaga : class, ISaga
        {
            return new RetrySagaConsumeContext<TSaga>(context, retryPolicy, retryContext);
        }

        public static void UseRetry<T>(this IPipeConfigurator<T> configurator, Action<IRetryConfigurator> configure)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new RetryPipeSpecification<T>();

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry(this IPipeConfigurator<ConsumeContext> configurator, IBusFactoryConfigurator connector,
            Action<IRetryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new RetryBusObserver();
            connector.ConnectBusObserver(observer);

            var specification = new ConsumeContextRetryPipeSpecification(observer.Stopping);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry(this IBusFactoryConfigurator configurator, Action<IRetryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new RetryBusObserver();
            configurator.ConnectBusObserver(observer);

            var specification = new ConsumeContextRetryPipeSpecification(observer.Stopping);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IBusFactoryConfigurator connector,
            Action<IRetryConfigurator> configure)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new RetryBusObserver();
            connector.ConnectBusObserver(observer);

            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<T>, RetryConsumeContext<T>>(Factory, observer.Stopping);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry<TConsumer>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, IBusFactoryConfigurator connector,
            Action<IRetryConfigurator> configure)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new RetryBusObserver();
            connector.ConnectBusObserver(observer);

            var specification =
                new ConsumeContextRetryPipeSpecification<ConsumerConsumeContext<TConsumer>, RetryConsumerConsumeContext<TConsumer>>(Factory, observer.Stopping);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry<TSaga>(this IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, IBusFactoryConfigurator connector,
            Action<IRetryConfigurator> configure)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new RetryBusObserver();
            connector.ConnectBusObserver(observer);

            var specification = new ConsumeContextRetryPipeSpecification<SagaConsumeContext<TSaga>, RetrySagaConsumeContext<TSaga>>(Factory, observer.Stopping);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Create an immediate retry policy with the specified number of retries, with no
        /// delay between attempts.
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IRetryConfigurator None(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new NoRetryPolicy(filter));

            return configurator;
        }

        /// <summary>
        /// Create an immediate retry policy with the specified number of retries, with no
        /// delay between attempts.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryLimit">The number of retries to attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Immediate(this IRetryConfigurator configurator, int retryLimit)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new ImmediateRetryPolicy(filter, retryLimit));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Intervals(this IRetryConfigurator configurator, params TimeSpan[] intervals)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, intervals));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Intervals(this IRetryConfigurator configurator, params int[] intervals)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, intervals));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified number of retries at a fixed interval
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <param name="interval">The interval between each retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Interval(this IRetryConfigurator configurator, int retryCount, TimeSpan interval)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, Enumerable.Repeat(interval, retryCount).ToArray()));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified number of retries at a fixed interval
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <param name="interval">The interval between each retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Interval(this IRetryConfigurator configurator, int retryCount, int interval)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, Enumerable.Repeat(interval, retryCount).ToArray()));

            return configurator;
        }

        /// <summary>
        /// Create an exponential retry policy with the specified number of retries at exponential
        /// intervals
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryLimit"></param>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <param name="intervalDelta"></param>
        /// <returns></returns>
        public static IRetryConfigurator Exponential(this IRetryConfigurator configurator, int retryLimit, TimeSpan minInterval, TimeSpan maxInterval,
            TimeSpan intervalDelta)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new ExponentialRetryPolicy(filter, retryLimit, minInterval, maxInterval, intervalDelta));

            return configurator;
        }

        /// <summary>
        /// Create an incremental retry policy with the specified number of retry attempts with an incrementing
        /// interval between retries
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryLimit">The number of retry attempts</param>
        /// <param name="initialInterval">The initial retry interval</param>
        /// <param name="intervalIncrement">The interval to add to the retry interval with each subsequent retry</param>
        /// <returns></returns>
        public static IRetryConfigurator Incremental(this IRetryConfigurator configurator, int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IncrementalRetryPolicy(filter, retryLimit, initialInterval, intervalIncrement));

            return configurator;
        }
    }
}
