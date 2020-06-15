namespace MassTransit.JobService.Pipeline
{
    using System;
    using ConsumeConnectors;
    using Metadata;


    public class JobMessageConnectorFactory<TConsumer, TJob> :
        IMessageConnectorFactory
        where TConsumer : class, IJobConsumer<TJob>
        where TJob : class
    {
        readonly IConsumerMessageConnector<TConsumer> _jobConsumerConnector;

        public JobMessageConnectorFactory()
        {
            _jobConsumerConnector = new JobConsumerMessageConnector<TConsumer, TJob>();
        }

        public IConsumerMessageConnector<T> CreateConsumerConnector<T>()
            where T : class
        {
            if (_jobConsumerConnector is IConsumerMessageConnector<T> connector)
                return connector;

            throw new ArgumentException("The consumer type did not match the connector type");
        }

        IInstanceMessageConnector<T> IMessageConnectorFactory.CreateInstanceConnector<T>()
        {
            throw new NotSupportedException($"{TypeMetadataCache<TJob>.ShortName} jobs cannot be connected to consumer instances.");
        }
    }
}
