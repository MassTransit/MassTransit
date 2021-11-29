namespace MassTransit
{
    using System;
    using Configuration;


    public static class TimeoutConfiguratorExtensions
    {
        /// <summary>
        /// Cancels context's CancellationToken once timeout is reached.
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure timeout</param>
        public static void UseTimeout<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<ITimeoutConfigurator> configure = default)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new TimeoutSpecification<T>();

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Cancels context's CancellationToken once timeout is reached.
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure timeout</param>
        public static void UseTimeout(this IConsumePipeConfigurator configurator, Action<ITimeoutConfigurator> configure = default)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new TimeoutConfigurationObserver(configurator, configure);
        }

        /// <summary>
        /// Cancels context's CancellationToken once timeout is reached.
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure timeout</param>
        public static void UseTimeout<TConsumer>(this IConsumerConfigurator<TConsumer> configurator, Action<ITimeoutConfigurator> configure = default)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new TimeoutConsumerConfigurationObserver<TConsumer>(configurator, configure);
            configurator.ConnectConsumerConfigurationObserver(observer);
        }

        /// <summary>
        /// Cancels context's CancellationToken once timeout is reached.
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure timeout</param>
        public static void UseTimeout<TSaga>(this ISagaConfigurator<TSaga> configurator, Action<ITimeoutConfigurator> configure = default)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new TimeoutSagaConfigurationObserver<TSaga>(configurator, configure);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Cancels context's CancellationToken once timeout is reached.
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure timeout</param>
        public static void UseTimeout<TMessage>(this IHandlerConfigurator<TMessage> configurator, Action<ITimeoutConfigurator> configure = default)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new TimeoutHandlerConfigurationObserver(configure);
            configurator.ConnectHandlerConfigurationObserver(observer);
        }
    }
}
