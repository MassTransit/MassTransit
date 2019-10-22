namespace GreenPipes
{
    using System;
    using System.Text;
    using MassTransit.Courier;
    using Partitioning;
    using Specifications;


    public static class CompensateActivityPartitionerExtensions
    {
        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
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

            byte[] PartitionKeyProvider(CompensateActivityContext<TActivity, TLog> context) => keyProvider(context).ToByteArray();

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
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

            byte[] PartitionKeyProvider(CompensateActivityContext<TActivity, TLog> context) => keyProvider(context).ToByteArray();

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
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
                var key = keyProvider(context) ?? "";
                return textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
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
                var key = keyProvider(context) ?? "";
                return textEncoding.GetBytes(key);
            }

            var specification = new PartitionerPipeSpecification<CompensateActivityContext<TActivity, TLog>>(PartitionKeyProvider, partitioner);

            configurator.AddPipeSpecification(specification);
        }
    }
}
