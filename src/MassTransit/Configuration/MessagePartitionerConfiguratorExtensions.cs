namespace MassTransit
{
    using System;
    using GreenPipes.Partitioning;
    using PipeConfigurators;


    public static class MessagePartitionerConfiguratorExtensions
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
    }
}
