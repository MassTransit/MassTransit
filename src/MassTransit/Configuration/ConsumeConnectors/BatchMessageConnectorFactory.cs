namespace MassTransit.ConsumeConnectors
{
    using System;
    using Metadata;


    public class BatchMessageConnectorFactory<TConsumer, TMessage> :
        IMessageConnectorFactory
        where TConsumer : class, IConsumer<Batch<TMessage>>
        where TMessage : class
    {
        readonly BatchConsumerMessageConnector<TConsumer, TMessage> _consumerConnector;

        public BatchMessageConnectorFactory()
        {
            _consumerConnector = new BatchConsumerMessageConnector<TConsumer, TMessage>();
        }

        public IConsumerMessageConnector<T> CreateConsumerConnector<T>()
            where T : class
        {
            if (_consumerConnector is IConsumerMessageConnector<T> result)
                return result;

            throw new ArgumentException("The consumer type did not match the connector type");
        }

        IInstanceMessageConnector<T> IMessageConnectorFactory.CreateInstanceConnector<T>()
        {
            throw new NotSupportedException($"Batch<{TypeMetadataCache<TMessage>.ShortName}> cannot be connected to a consumer instance.");
        }
    }
}
