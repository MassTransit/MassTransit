using System.Threading;
using MassTransit.Context;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbMessageSendContext<T> :
        MessageSendContext<T>,
        EventStoreDbSendContext<T>
        where T : class
    {
        public EventStoreDbMessageSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {

        }

        public StreamName StreamName { get; set; }
    }
}
