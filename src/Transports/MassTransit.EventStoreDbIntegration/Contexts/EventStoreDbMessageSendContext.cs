using System;
using System.Net.Mime;
using System.Threading;
using MassTransit.Context;
using MassTransit.Serialization;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbMessageSendContext<T> :
        MessageSendContext<T>,
        EventStoreDbSendContext<T>
        where T : class
    {
        string _esdbContentType;

        public EventStoreDbMessageSendContext(string streamName, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            StreamName = streamName;
        }

        public string StreamName { get; set; }

        public string EventStoreDBContentType
        {
            get => _esdbContentType;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _esdbContentType = value.Equals(JsonMessageSerializer.ContentTypeHeaderValue) || value.Equals(MediaTypeNames.Application.Json)
                    ? MediaTypeNames.Application.Json
                    : MediaTypeNames.Application.Octet;
            }
        }
    }
}
