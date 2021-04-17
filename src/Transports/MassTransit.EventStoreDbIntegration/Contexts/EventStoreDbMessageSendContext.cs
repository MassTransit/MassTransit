using System.Net.Mime;
using System.Threading;
using MassTransit.Context;
using MassTransit.Serialization;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbMessageSendContext<T> :
        MessageSendContext<T>,
        EventStoreDbSendContext<T>
        where T : class
    {
        string _esdbContentType = null;

        public EventStoreDbMessageSendContext(string streamName, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            StreamName = streamName;
        }

        public string StreamName { get; set; }

        public string EventStoreDBContentType
        {
            get
            {
                if (_esdbContentType == null)
                {
                    _esdbContentType = ContentType.MediaType.Equals(JsonMessageSerializer.ContentTypeHeaderValue)
                        || ContentType.MediaType.Equals(MediaTypeNames.Application.Json)
                        ? MediaTypeNames.Application.Json
                        : MediaTypeNames.Application.Octet;
                }

                return _esdbContentType;
            }
        }
    }
}
