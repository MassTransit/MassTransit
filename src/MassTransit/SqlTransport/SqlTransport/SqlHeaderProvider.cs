#nullable enable
namespace MassTransit.SqlTransport
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Transports;


    public class SqlHeaderProvider :
        IHeaderProvider
    {
        readonly SqlTransportMessage _message;

        public SqlHeaderProvider(SqlTransportMessage message)
        {
            _message = message;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _message.GetHeaders().GetAll().Concat(_message.GetTransportHeaders().GetAll());
        }

        public bool TryGetHeader(string key, [NotNullWhen(true)] out object? value)
        {
            switch (key)
            {
                case MessageHeaders.ContentType:
                    value = _message.ContentType;
                    return value != null;
                case MessageHeaders.MessageType:
                    value = _message.MessageType;
                    return value != null;
                case MessageHeaders.MessageId:
                    value = _message.MessageId;
                    return value != null;
                case MessageHeaders.CorrelationId:
                    value = _message.CorrelationId;
                    return value != null;
                case MessageHeaders.ConversationId:
                    value = _message.ConversationId;
                    return value != null;
                case MessageHeaders.RequestId:
                    value = _message.RequestId;
                    return value != null;
                case MessageHeaders.InitiatorId:
                    value = _message.InitiatorId;
                    return value != null;
                case MessageHeaders.SourceAddress:
                    value = _message.SourceAddress;
                    return value != null;
                case MessageHeaders.ResponseAddress:
                    value = _message.ResponseAddress;
                    return value != null;
                case MessageHeaders.FaultAddress:
                    value = _message.FaultAddress;
                    return value != null;
                case MessageHeaders.TransportMessageId:
                    value = _message.TransportMessageId;
                    return true;
                case nameof(_message.MessageDeliveryId):
                    value = _message.MessageDeliveryId;
                    return true;
                case nameof(_message.DeliveryCount):
                    value = _message.DeliveryCount;
                    return true;
                case nameof(_message.RoutingKey):
                    value = _message.RoutingKey;
                    return value != null;
                case nameof(_message.PartitionKey):
                    value = _message.PartitionKey;
                    return value != null;
            }

            if (_message.GetTransportHeaders().TryGetHeader(key, out value))
                return true;

            if (_message.GetHeaders().TryGetHeader(key, out value))
                return true;

            value = default;
            return false;
        }
    }
}
