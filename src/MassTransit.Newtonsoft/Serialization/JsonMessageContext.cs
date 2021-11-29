namespace MassTransit.Serialization
{
    using System;
    using JsonConverters;
    using Newtonsoft.Json;


    /// <summary>
    /// While not used by MassTransit, this helper class can be used to deserialize a JSON message.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonMessageContext<T> :
        MessageContext
        where T : class
    {
        [JsonProperty("message")]
        public T Message { get; set; }

        [JsonProperty("messageId")]
        public Guid? MessageId { get; set; }

        [JsonProperty("requestId")]
        public Guid? RequestId { get; set; }

        [JsonProperty("correlationId")]
        public Guid? CorrelationId { get; set; }

        [JsonProperty("conversationId")]
        public Guid? ConversationId { get; set; }

        [JsonProperty("initiatorId")]
        public Guid? InitiatorId { get; set; }

        [JsonProperty("expirationTime")]
        public DateTime? ExpirationTime { get; set; }

        [JsonProperty("sourceAddress")]
        public Uri SourceAddress { get; set; }

        [JsonProperty("destinationAddress")]
        public Uri DestinationAddress { get; set; }

        [JsonProperty("responseAddress")]
        public Uri ResponseAddress { get; set; }

        [JsonProperty("faultAddress")]
        public Uri FaultAddress { get; set; }

        [JsonProperty("sentTime")]
        public DateTime? SentTime { get; set; }

        [JsonProperty("headers")]
        [JsonConverter(typeof(JsonMessageContextHeaderConverter))]
        public Headers Headers { get; set; }

        [JsonProperty("host")]
        [JsonConverter(typeof(JsonMessageContextHostInfoConverter))]
        public HostInfo Host { get; set; }
    }
}
