namespace MassTransit.Configuration
{
    using System;


    public interface IMessageInterfaceType
    {
        Type MessageType { get; }

        IConsumerMessageConnector<T> GetConsumerConnector<T>()
            where T : class;

        IInstanceMessageConnector<T> GetInstanceConnector<T>()
            where T : class;
    }
}
