#nullable enable
namespace MassTransit.Logging
{
    public static class DiagnosticHeaders
    {
        public const string DefaultListenerName = "MassTransit";

        public const string DiagnosticId = "Diagnostic-Id";
        public const string ActivityId = "MT-Activity-Id";
        public const string ActivityCorrelationContext = "MT-Activity-Correlation-Context";

        public const string MessageId = "messaging.masstransit.message_id";
        public const string CorrelationId = "messaging.masstransit.correlation_id";
        public const string InitiatorId = "messaging.masstransit.initiator_id";
        public const string RequestId = "messaging.masstransit.request_id";
        public const string SourceAddress = "messaging.masstransit.source_address";
        public const string DestinationAddress = "messaging.masstransit.destination_address";
        public const string InputAddress = "messaging.masstransit.input_address";
        public const string TrackingNumber = "messaging.masstransit.tracking_number";

        public const string MessageTypes = "messaging.masstransit.message_types";

        public const string PeerAddress = "peer.address";
        public const string ServiceName = "service.name";

        public const string BeginState = "messaging.masstransit.begin_state";
        public const string EndState = "messaging.masstransit.end_state";
        public const string SagaId = "messaging.masstransit.saga_id";


        public static class Messaging
        {
            public const string BodyLength = "messaging.message_payload_size_bytes";
            public const string ConversationId = "messaging.conversation_id";
            public const string Destination = "messaging.destination";
            public const string DestinationKind = "messaging.destination_kind";
            public const string TransportMessageId = "messaging.message_id";
            public const string Operation = "messaging.operation";
            public const string System = "messaging.system";


            public static class RabbitMq
            {
                public const string RoutingKey = "messaging.rabbitmq.routing_key";
            }
        }
    }
}
