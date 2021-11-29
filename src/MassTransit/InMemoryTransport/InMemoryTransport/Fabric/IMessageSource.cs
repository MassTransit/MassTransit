namespace MassTransit.InMemoryTransport.Fabric
{
    using System.Collections.Generic;


    public interface IMessageSource<T>
        where T : class
    {
        IEnumerable<IMessageSink<InMemoryTransportMessage>> Sinks { get; }
        void Connect(IMessageSink<T> sink);
    }
}
