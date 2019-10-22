namespace GreenPipes
{
    using System;
    using Configurators;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.PipeConfigurators;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Saga;


    public static class RetryPipeConfiguratorExtensions
    {
        [Obsolete("Use of the lambda-based policy configurator is recommended")]
        public static void UseRetry(this IPipeConfigurator<ConsumeContext> configurator, IRetryPolicy retryPolicy)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification();

            specification.SetRetryPolicy(x => retryPolicy);

            configurator.AddPipeSpecification(specification);
        }

        public static void UseRetry(this IPipeConfigurator<ConsumeContext> configurator, Action<IRetryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification();

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

        [Obsolete("Use of the lambda-based policy configurator is recommended")]
        public static void UseRetry<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IRetryPolicy retryPolicy)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<T>, RetryConsumeContext<T>>(Factory);

            specification.SetRetryPolicy(x => retryPolicy);

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

        static RetryConsumeContext<T> Factory<T>(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where T : class
        {
            return new RetryConsumeContext<T>(context, retryPolicy, retryContext);
        }

        [Obsolete("Use of the lambda-based policy configurator is recommended")]
        public static void UseRetry<TConsumer>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, IRetryPolicy retryPolicy)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification<ConsumerConsumeContext<TConsumer>, RetryConsumerConsumeContext<TConsumer>>(Factory);

            specification.SetRetryPolicy(x => retryPolicy);

            configurator.AddPipeSpecification(specification);
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

        static RetryConsumerConsumeContext<TConsumer> Factory<TConsumer>(ConsumerConsumeContext<TConsumer> context, IRetryPolicy retryPolicy,
            RetryContext retryContext)
            where TConsumer : class
        {
            return new RetryConsumerConsumeContext<TConsumer>(context, retryPolicy, retryContext);
        }

        [Obsolete("Use of the lambda-based policy configurator is recommended")]
        public static void UseRetry<TSaga>(this IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, IRetryPolicy retryPolicy)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumeContextRetryPipeSpecification<SagaConsumeContext<TSaga>, RetrySagaConsumeContext<TSaga>>(Factory);

            specification.SetRetryPolicy(x => retryPolicy);

            configurator.AddPipeSpecification(specification);
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

        static RetrySagaConsumeContext<TSaga> Factory<TSaga>(SagaConsumeContext<TSaga> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where TSaga : class, ISaga
        {
            return new RetrySagaConsumeContext<TSaga>(context, retryPolicy, retryContext);
        }
    }
}
