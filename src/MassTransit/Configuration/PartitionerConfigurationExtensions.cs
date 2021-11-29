namespace MassTransit
{
    using System;
    using System.Text;
    using Configuration;
    using Courier;
    using Middleware;


    public static class PartitionerConfigurationExtensions
    {
        /// <summary>
        /// Adds partitioning to the consume pipeline, with a number of partitions handling all message types on the receive endpoint. Endpoints must have
        /// a CorrelationId provider available, which can be specified using GlobalTopology.Send.UseCorrelationId&lt;T&gt;(x => x.SomeId);
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="partitionCount">The number of partitions</param>
        public static void UseMessagePartitioner(this IConsumePipeConfigurator configurator, int partitionCount)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var partitioner = new Partitioner(partitionCount, new Murmur3UnsafeHashGenerator());

            var observer = new PartitionMessageConfigurationObserver(configurator, partitioner);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
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

            byte[] PartitionKeyProvider(ConsumeContext<T> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<ConsumeContext<T>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner that is shared</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IPartitioner partitioner,
            Func<ConsumeContext<T>, Guid> keyProvider)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(ConsumeContext<T> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<ConsumeContext<T>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, int partitionCount,
            Func<ConsumeContext<T>, Guid> keyProvider)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(ConsumeContext<T> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var partitioner = new Partitioner(partitionCount, new Murmur3UnsafeHashGenerator());

            var specification = new PartitionerPipeSpecification<ConsumeContext<T>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
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
        /// Adds a partition filter, which also limits concurrency by the partition count.
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
        /// Adds a partition filter, which also limits concurrency by the partition count.
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
        /// Adds a partition filter, which also limits concurrency by the partition count.
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

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            int partitionCount, Func<ExecuteActivityContext<TActivity, TArguments>, Guid> keyProvider)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(ExecuteActivityContext<TActivity, TArguments> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner to share</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            IPartitioner partitioner, Func<ExecuteActivityContext<TActivity, TArguments>, Guid> keyProvider)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(ExecuteActivityContext<TActivity, TArguments> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding">The text encoding to use to convert the string to byte[] (defaults to UTF8)</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            int partitionCount, Func<ExecuteActivityContext<TActivity, TArguments>, string> keyProvider, Encoding encoding = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(ExecuteActivityContext<TActivity, TArguments> context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner to share</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding">The text encoding to use to convert the string to byte[] (defaults to UTF8)</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            IPartitioner partitioner, Func<ExecuteActivityContext<TActivity, TArguments>, string> keyProvider, Encoding encoding = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(ExecuteActivityContext<TActivity, TArguments> context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TActivity, TLog>(this IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator,
            int partitionCount, Func<CompensateActivityContext<TActivity, TLog>, Guid> keyProvider)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(CompensateActivityContext<TActivity, TLog> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner to share</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TActivity, TLog>(this IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator,
            IPartitioner partitioner, Func<CompensateActivityContext<TActivity, TLog>, Guid> keyProvider)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(CompensateActivityContext<TActivity, TLog> context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding">The text encoding to use to convert the string to byte[] (defaults to UTF8)</param>
        public static void UsePartitioner<TActivity, TLog>(this IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator,
            int partitionCount, Func<CompensateActivityContext<TActivity, TLog>, string> keyProvider, Encoding encoding = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(CompensateActivityContext<TActivity, TLog> context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner to share</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding">The text encoding to use to convert the string to byte[] (defaults to UTF8)</param>
        public static void UsePartitioner<TActivity, TLog>(this IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator,
            IPartitioner partitioner, Func<CompensateActivityContext<TActivity, TLog>, string> keyProvider, Encoding encoding = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(CompensateActivityContext<TActivity, TLog> context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Create a partitioner which can be used across multiple partitioner filters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <param name="partitionCount"></param>
        /// <returns></returns>
        public static IPartitioner CreatePartitioner<T>(this IPipeConfigurator<T> _, int partitionCount)
            where T : class, PipeContext
        {
            return new Partitioner(partitionCount, new Murmur3UnsafeHashGenerator());
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, int partitionCount, Func<T, Guid> keyProvider)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(T context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner that is shared</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, Guid> keyProvider)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(T context)
            {
                return keyProvider(context).ToByteArray();
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding"></param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, int partitionCount, Func<T, string> keyProvider, Encoding encoding = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(T context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner that is shared</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding"></param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, string> keyProvider,
            Encoding encoding = null)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            byte[] PartitionKeyProvider(T context)
            {
                var key = keyProvider(context);
                return key == null
                    ? Array.Empty<byte>()
                    : textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, int partitionCount, Func<T, long> keyProvider)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(T context)
            {
                var key = keyProvider(context);
                return BitConverter.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner that is shared</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, long> keyProvider)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(T context)
            {
                var key = keyProvider(context);
                return BitConverter.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, int partitionCount, Func<T, byte[]> keyProvider)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(T context)
            {
                return keyProvider(context) ?? Array.Empty<byte>();
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a partition filter, which also limits concurrency by the partition count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner that is shared</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<T>(this IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, byte[]> keyProvider)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            byte[] PartitionKeyProvider(T context)
            {
                return keyProvider(context) ?? Array.Empty<byte>();
            }

            var specification = new PartitionerPipeSpecification<T>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }
    }
}
