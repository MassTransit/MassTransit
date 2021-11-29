namespace MassTransit.Serialization
{
    public static class NServiceBusMessageHeaders
    {
        public const string ContentType = "NServiceBus.ContentType";
        public const string ConversationId = "NServiceBus.ConversationId";
        public const string CorrelationId = "NServiceBus.CorrelationId";
        public const string EnclosedMessageTypes = "NServiceBus.EnclosedMessageTypes";
        public const string MessageId = "NServiceBus.MessageId";
        public const string MessageIntent = "NServiceBus.MessageIntent";
        public const string OriginatingEndpoint = "NServiceBus.OriginatingEndpoint";
        public const string OriginatingMachine = "NServiceBus.OriginatingMachine";
        public const string ReplyToAddress = "NServiceBus.ReplyToAddress";
        public const string TimeSent = "NServiceBus.TimeSent";
        public const string Version = "NServiceBus.Version";

        public const string DiagnosticsOriginatingHostId = "$.diagnostics.originating.hostid";
        public const string TransportRabbitMqConfirmationId = "NServiceBus.Transport.RabbitMQ.ConfirmationId";
    }
}
