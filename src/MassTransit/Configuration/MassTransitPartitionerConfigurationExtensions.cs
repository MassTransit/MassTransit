namespace GreenPipes
{
    using System;
    using MassTransit;
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
    }
}
