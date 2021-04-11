using System.Threading;
using MassTransit.Context;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbMessageSendContext<T> :
        MessageSendContext<T>,
        EventStoreDbSendContext<T>
        where T : class
    {
        public EventStoreDbMessageSendContext(string streamName, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            StreamName = streamName;
        }

        public string StreamName { get; set; }
    }
}
