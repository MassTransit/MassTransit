namespace MassTransit
{
    using System;
    using Configuration;
    using Contracts;
    using Middleware;


    public static class ConcurrentMessageLimitExtensions
    {
        /// <summary>
        /// Limits the number of concurrent messages consumed by the consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for all message types for the consumer</param>
        public static void UseConcurrentMessageLimit<TConsumer>(this IConsumerConfigurator<TConsumer> configurator, int concurrentMessageLimit)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new ConcurrencyLimitConsumerConfigurationObserver<TConsumer>(configurator, concurrentMessageLimit);
            configurator.ConnectConsumerConfigurationObserver(observer);
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed by the consumer, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for all message types for the consumer</param>
        /// <param name="managementEndpointConfigurator">A management endpoint configurator to support runtime adjustment</param>
        /// <param name="id">An identifier for the concurrency limit to allow selective adjustment</param>
        public static void UseConcurrentMessageLimit<TConsumer>(this IConsumerConfigurator<TConsumer> configurator, int concurrentMessageLimit,
            IReceiveEndpointConfigurator managementEndpointConfigurator, string id = null)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (managementEndpointConfigurator == null)
                throw new ArgumentNullException(nameof(managementEndpointConfigurator));

            var observer = new ConcurrencyLimitConsumerConfigurationObserver<TConsumer>(configurator, concurrentMessageLimit, id);
            configurator.ConnectConsumerConfigurationObserver(observer);

            managementEndpointConfigurator.Instance(observer.Limiter, x =>
            {
                x.UseConcurrentMessageLimit(1);
                x.Message<SetConcurrencyLimit>(m => m.UseRetry(r => r.None()));
            });
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed by the saga, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for all message types for the saga</param>
        public static void UseConcurrentMessageLimit<TSaga>(this ISagaConfigurator<TSaga> configurator, int concurrentMessageLimit)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new ConcurrencyLimitSagaConfigurationObserver<TSaga>(configurator, concurrentMessageLimit);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed by the saga, regardless of message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for all message types for the saga</param>
        /// <param name="managementEndpointConfigurator">A management endpoint configurator to support runtime adjustment</param>
        /// <param name="id">An identifier for the concurrency limit to allow selective adjustment</param>
        public static void UseConcurrentMessageLimit<TSaga>(this ISagaConfigurator<TSaga> configurator, int concurrentMessageLimit,
            IReceiveEndpointConfigurator managementEndpointConfigurator, string id = null)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new ConcurrencyLimitSagaConfigurationObserver<TSaga>(configurator, concurrentMessageLimit, id);
            configurator.ConnectSagaConfigurationObserver(observer);

            managementEndpointConfigurator.Instance(observer.Limiter, x =>
            {
                x.UseConcurrentMessageLimit(1);
                x.Message<SetConcurrencyLimit>(m => m.UseRetry(r => r.None()));
            });
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed by the handler.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for the handler message type</param>
        public static void UseConcurrentMessageLimit<TMessage>(this IHandlerConfigurator<TMessage> configurator, int concurrentMessageLimit)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new ConcurrencyLimitHandlerConfigurationObserver(concurrentMessageLimit);
            configurator.ConnectHandlerConfigurationObserver(observer);
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed by the handler.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for the handler message type</param>
        /// <param name="managementEndpointConfigurator">A management endpoint configurator to support runtime adjustment</param>
        /// <param name="id">An identifier for the concurrency limit to allow selective adjustment</param>
        public static void UseConcurrentMessageLimit<TMessage>(this IHandlerConfigurator<TMessage> configurator, int concurrentMessageLimit,
            IReceiveEndpointConfigurator managementEndpointConfigurator, string id = null)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new ConcurrencyLimitHandlerConfigurationObserver(concurrentMessageLimit, id);
            configurator.ConnectHandlerConfigurationObserver(observer);

            managementEndpointConfigurator.Instance(observer.Limiter, x =>
            {
                x.UseConcurrentMessageLimit(1);
                x.Message<SetConcurrencyLimit>(m => m.UseRetry(r => r.None()));
            });
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed for the specified message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for the message type</param>
        public static void UseConcurrentMessageLimit<TMessage>(this IPipeConfigurator<ConsumeContext<TMessage>> configurator, int concurrentMessageLimit)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var limiter = new ConcurrencyLimiter(concurrentMessageLimit);

            var specification = new ConcurrencyLimitConsumePipeSpecification<TMessage>(limiter);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Limits the number of concurrent messages consumed for the specified message type.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrentMessageLimit">The concurrent message limit for the message type</param>
        /// <param name="managementEndpointConfigurator">A management endpoint configurator to support runtime adjustment</param>
        /// <param name="id">An identifier for the concurrency limit to allow selective adjustment</param>
        public static void UseConcurrentMessageLimit<TMessage>(this IPipeConfigurator<ConsumeContext<TMessage>> configurator, int concurrentMessageLimit,
            IReceiveEndpointConfigurator managementEndpointConfigurator, string id = null)
            where TMessage : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);

            var specification = new ConcurrencyLimitConsumePipeSpecification<TMessage>(limiter);

            configurator.AddPipeSpecification(specification);

            managementEndpointConfigurator.Instance(limiter, x =>
            {
                x.UseConcurrentMessageLimit(1);
                x.Message<SetConcurrencyLimit>(m => m.UseRetry(r => r.None()));
            });
        }
    }
}
