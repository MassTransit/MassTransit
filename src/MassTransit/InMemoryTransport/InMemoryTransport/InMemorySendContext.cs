#nullable enable
namespace MassTransit.InMemoryTransport
{
    using System.Threading;
    using Context;


    public class InMemorySendContext<T> :
        MessageSendContext<T>,
        RoutingKeySendContext
        where T : class
    {
        public InMemorySendContext(T message, CancellationToken cancellationToken = default)
            : base(message, cancellationToken)
        {
        }

        public string? RoutingKey { get; set; }
    }
}
