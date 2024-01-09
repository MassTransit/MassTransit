#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Serialization;


    public class SqlTransportMessage
    {
        SendHeaders? _headers;
        SendHeaders? _transportHeaders;

        public Guid TransportMessageId { get; set; }
        public string QueueName { get; set; } = null!;
        public short Priority { get; set; }
        public long MessageDeliveryId { get; set; }
        public Guid? ConsumerId { get; set; }
        public Guid? LockId { get; set; }
        public DateTime EnqueueTime { get; set; }
        public int DeliveryCount { get; set; }

        public string? PartitionKey { get; set; }
        public string? RoutingKey { get; set; }

        public string? TransportHeaders { get; set; }

        public string? ContentType { get; set; }
        public string? MessageType { get; set; }
        public string? Body { get; set; }
        public byte[]? BinaryBody { get; set; }

        public string? Headers { get; set; }
        public string? Host { get; set; }

        public Guid? MessageId { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? InitiatorId { get; set; }

        public DateTime? ExpirationTime { get; set; }

        public Uri? SourceAddress { get; set; }
        public Uri? DestinationAddress { get; set; }
        public Uri? ResponseAddress { get; set; }
        public Uri? FaultAddress { get; set; }

        public DateTime? SentTime { get; set; }

        public SendHeaders GetHeaders()
        {
            return _headers ??= DeserializeHeaders(Headers);
        }

        public SendHeaders GetTransportHeaders()
        {
            return _transportHeaders ??= DeserializeHeaders(TransportHeaders);
        }

        public static SendHeaders DeserializeHeaders(string? jsonHeaders)
        {
            var headers = new DictionarySendHeaders();

            if (jsonHeaders != null)
            {
                var elements = JsonSerializer.Deserialize<IEnumerable<KeyValuePair<string, object>>>(jsonHeaders, SystemTextJsonMessageSerializer.Options);
                if (elements != null)
                {
                    foreach (KeyValuePair<string, object> element in elements)
                        headers.Set(element.Key, element.Value);
                }
            }

            return headers;
        }

        HostInfo? GetHost()
        {
            if (Host == null)
                return null;

            return JsonSerializer.Deserialize<HostInfo>(Host, SystemTextJsonMessageSerializer.Options);
        }

        static Uri? ToUri(string? value)
        {
            try
            {
                return string.IsNullOrWhiteSpace(value)
                    ? null
                    : new Uri(value);
            }
            catch (FormatException)
            {
                return default;
            }
        }
    }
}
