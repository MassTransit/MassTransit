namespace MassTransit.ConsumeConnectors
{
    using System;


    /// <summary>
    /// A batch consumer
    /// </summary>
    public class BatchConsumerInterfaceType :
        IMessageInterfaceType
    {
        readonly Lazy<IMessageConnectorFactory> _consumeConnectorFactory;

        public BatchConsumerInterfaceType(Type messageType, Type consumerType)
        {
            MessageType = messageType;

            _consumeConnectorFactory = new Lazy<IMessageConnectorFactory>(() => (IMessageConnectorFactory)
                Activator.CreateInstance(typeof(BatchMessageConnectorFactory<,>).MakeGenericType(consumerType, messageType)));
        }

        public Type MessageType { get; }

        public IConsumerMessageConnector<T> GetConsumerConnector<T>()
            where T : class
        {
            return _consumeConnectorFactory.Value.CreateConsumerConnector<T>();
        }

        public IInstanceMessageConnector<T> GetInstanceConnector<T>()
            where T : class
        {
            return _consumeConnectorFactory.Value.CreateInstanceConnector<T>();
        }
    }
}
