namespace MassTransit.Configuration
{
    using System;


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
            return _consumerConnector as IConsumerMessageConnector<T> ?? throw new ArgumentException("The consumer type did not match the connector type");
        }

        public IInstanceMessageConnector<T> CreateInstanceConnector<T>()
            where T : class
        {
            throw new NotSupportedException($"Batch<{TypeCache<TMessage>.ShortName}> cannot be connected to a consumer instance.");
        }
    }
}
