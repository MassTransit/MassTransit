namespace MassTransit.Transports.InMemory.Fabric
{
    using System.Collections.Generic;


    public interface IMessageSource<T>
        where T : class
    {
        void Connect(IMessageSink<T> sink);

        IEnumerable<IMessageSink<InMemoryTransportMessage>> Sinks { get; }
    }
}