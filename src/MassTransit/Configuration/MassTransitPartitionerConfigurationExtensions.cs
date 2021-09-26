namespace GreenPipes
{
    using System;
    using System.Text;
    using MassTransit;
    using MassTransit.PipeConfigurators;
    using MassTransit.Saga;
    using Partitioning;
    using Specifications;


    public static class MassTransitPartitionerConfigurationExtensions
    {
        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner that is shared</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IConsumePipeConfigurator configurator, IPartitioner partitioner, Func<ConsumeContext<T>, Guid> keyProvider)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] Provider(ConsumeContext<T> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<ConsumeContext<T>>(Provider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TConsumer>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, int partitionCount,
            Func<ConsumerConsumeContext<TConsumer>, Guid> keyProvider)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(ConsumerConsumeContext<TConsumer> context)
            {
                return keyProvider(context).ToByteArray();
            }

            UseConsumerPartitioner(configurator, partitionCount, PartitionKeyProvider);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding"></param>
        public static void UsePartitioner<TConsumer>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, int partitionCount,
            Func<ConsumerConsumeContext<TConsumer>, string> keyProvider, Encoding encoding = null)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(ConsumerConsumeContext<TConsumer> context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            UseConsumerPartitioner(configurator, partitionCount, PartitionKeyProvider);
        }

        static void UseConsumerPartitioner<TConsumer>(IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, int partitionCount,
            PartitionKeyProvider<ConsumerConsumeContext<TConsumer>> keyProvider)
            where TConsumer : class
        {
            var partitioner = new Partitioner(partitionCount, new Murmur3UnsafeHashGenerator());

            var specification = new PartitionConsumerSpecification<TConsumer>(partitioner, keyProvider);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TSaga>(this IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, int partitionCount,
            Func<SagaConsumeContext<TSaga>, Guid> keyProvider)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(SagaConsumeContext<TSaga> context)
            {
                return keyProvider(context).ToByteArray();
            }

            UseSagaPartitioner(configurator, partitionCount, PartitionKeyProvider);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding"></param>
        public static void UsePartitioner<TSaga>(this IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, int partitionCount,
            Func<SagaConsumeContext<TSaga>, string> keyProvider, Encoding encoding = null)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(SagaConsumeContext<TSaga> context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            UseSagaPartitioner(configurator, partitionCount, PartitionKeyProvider);
        }

        static void UseSagaPartitioner<TSaga>(IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, int partitionCount,
            PartitionKeyProvider<SagaConsumeContext<TSaga>> keyProvider)
            where TSaga : class, ISaga
        {
            var partitioner = new Partitioner(partitionCount, new Murmur3UnsafeHashGenerator());

            var specification = new PartitionSagaSpecification<TSaga>(partitioner, keyProvider);

            configurator.AddPipeSpecification(specification);
        }
    }
}
