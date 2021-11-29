namespace MassTransit.Tests.Conventional
{
    using System;
    using MassTransit.Configuration;


    /// <summary>
    /// A legacy message-only consumer
    /// </summary>
    public class CustomConsumerInterfaceType :
        IMessageInterfaceType
    {
        readonly Lazy<IMessageConnectorFactory> _consumeConnectorFactory;

        public CustomConsumerInterfaceType(Type messageType, Type consumerType)
        {
            MessageType = messageType;

            _consumeConnectorFactory = new Lazy<IMessageConnectorFactory>(() => (IMessageConnectorFactory)
                Activator.CreateInstance(typeof(CustomConsumeConnectorFactory<,>).MakeGenericType(consumerType,
                    messageType)));
        }

        public Type MessageType { get; }

        IConsumerMessageConnector<T> IMessageInterfaceType.GetConsumerConnector<T>()
        {
            return _consumeConnectorFactory.Value.CreateConsumerConnector<T>();
        }

        IInstanceMessageConnector<T> IMessageInterfaceType.GetInstanceConnector<T>()
        {
            return _consumeConnectorFactory.Value.CreateInstanceConnector<T>();
        }
    }
}
