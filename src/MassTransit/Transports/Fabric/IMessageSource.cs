#nullable enable
namespace MassTransit.Transports.Fabric
{
    using System.Collections.Generic;


    public interface IMessageSource<T>
        where T : class
    {
        IEnumerable<IMessageSink<T>> Sinks { get; }

        ConnectHandle Connect(IMessageSink<T> sink, string? routingKey);
    }
}
