namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// A job consumer
    /// </summary>
    public class JobInterfaceType :
        IMessageInterfaceType
    {
        readonly Lazy<IMessageConnectorFactory> _consumeConnectorFactory;

        public JobInterfaceType(Type messageType, Type consumerType)
        {
            MessageType = messageType;

            _consumeConnectorFactory = new Lazy<IMessageConnectorFactory>(() => (IMessageConnectorFactory)
                Activator.CreateInstance(typeof(JobMessageConnectorFactory<,>).MakeGenericType(consumerType, messageType)));
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
