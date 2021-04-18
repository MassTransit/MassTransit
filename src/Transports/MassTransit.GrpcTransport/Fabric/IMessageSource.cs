namespace MassTransit.GrpcTransport.Fabric
{
    using System.Collections.Generic;


    public interface IMessageSource<T>
        where T : class
    {
        IEnumerable<IMessageSink<T>> Sinks { get; }
        void Connect(IMessageSink<T> sink);
    }
}
