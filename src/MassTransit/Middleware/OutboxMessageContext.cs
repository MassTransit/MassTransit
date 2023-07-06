#nullable enable
namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;


    public interface OutboxMessageContext :
        MessageContext
    {
        long SequenceNumber { get; }

        new Guid MessageId { get; }

        string ContentType { get; }

        string MessageType { get; }

        string Body { get; }

        IReadOnlyDictionary<string, object> Properties { get; }
    }
}
