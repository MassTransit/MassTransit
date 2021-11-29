namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// A standard asynchronous consumer message type, defined by IConsumer
    /// </summary>
    public class ConsumerInterfaceType :
        IMessageInterfaceType
    {
        readonly Lazy<IMessageConnectorFactory> _consumeConnectorFactory;

        public ConsumerInterfaceType(Type messageType, Type consumerType)
        {
            MessageType = messageType;

            _consumeConnectorFactory = new Lazy<IMessageConnectorFactory>(() => (IMessageConnectorFactory)
                Activator.CreateInstance(typeof(ConsumeMessageConnectorFactory<,>).MakeGenericType(consumerType,
                    messageType)));
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
