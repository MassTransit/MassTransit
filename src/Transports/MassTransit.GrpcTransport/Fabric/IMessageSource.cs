namespace MassTransit.GrpcTransport.Fabric
{
    using System.Collections.Generic;
    using GreenPipes;


    public interface IMessageSource<T>
        where T : class
    {
        IEnumerable<IMessageSink<T>> Sinks { get; }

        ConnectHandle Connect(IMessageSink<T> sink, string routingKey);
    }
}
