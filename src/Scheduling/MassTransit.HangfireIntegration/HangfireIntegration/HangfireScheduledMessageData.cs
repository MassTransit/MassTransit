namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Serialization;


    public class HangfireScheduledMessageData
    {
        public string? DestinationAddress { get; set; }
        public string? ContentType { get; set; }
        public string? ExpirationTime { get; set; }
        public string? SourceAddress { get; set; }
        public string? ResponseAddress { get; set; }
        public string? FaultAddress { get; set; }
        public string? Body { get; set; }
        public string? MessageId { get; set; }
        public string? RequestId { get; set; }
        public string? CorrelationId { get; set; }
        public string? ConversationId { get; set; }
        public string? InitiatorId { get; set; }
        public string? TokenId { get; set; }
        public string? HeadersAsJson { get; set; }

        public Uri Destination => new Uri(DestinationAddress);

        protected static void SetBaseProperties(HangfireScheduledMessageData data, ConsumeContext context, Uri destination, MessageBody messageBody,
            Guid? tokenId = default)
        {
            data.DestinationAddress = destination?.ToString() ?? "";
            data.Body = messageBody.GetString();
            data.ContentType = context.ReceiveContext.ContentType.ToString();
            data.FaultAddress = context.FaultAddress?.ToString() ?? "";
            data.ResponseAddress = context.ResponseAddress?.ToString() ?? "";

            if (context.MessageId.HasValue)
                data.MessageId = context.MessageId.Value.ToString();

            if (context.CorrelationId.HasValue)
                data.CorrelationId = context.CorrelationId.Value.ToString();

            if (context.ConversationId.HasValue)
                data.ConversationId = context.ConversationId.Value.ToString();

            if (context.InitiatorId.HasValue)
                data.InitiatorId = context.InitiatorId.Value.ToString();

            if (context.RequestId.HasValue)
                data.RequestId = context.RequestId.Value.ToString();

            if (context.ExpirationTime.HasValue)
                data.ExpirationTime = context.ExpirationTime.Value.ToString("O");

            if (tokenId.HasValue)
                data.TokenId = tokenId.Value.ToString();

            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll().ToList();
            if (headers.Any())
                data.HeadersAsJson = JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options);
        }
    }
}
