#nullable enable
namespace MassTransit.Logging
{
    public static class DiagnosticHeaders
    {
        public const string DefaultListenerName = "MassTransit";

        public const string ActivityId = "MT-Activity-Id";
        public const string ActivityCorrelationContext = "MT-Activity-Correlation-Context";

        public const string CorrelationId = "masstransit.correlation_id";
        public const string InitiatorId = "masstransit.initiator_id";
        public const string SourceAddress = "masstransit.source_address";
        public const string DestinationAddress = "masstransit.destination_address";
        public const string InputAddress = "masstransit.input_address";
        public const string TrackingNumber = "masstransit.tracking_number";

        public const string SourceHostMachine = "source_host_machine";
        public const string MessageTypes = "message_types";

        public const string PeerAddress = "peer.address";
        public const string PeerHost = "peer.host";
        public const string PeerService = "peer.service";
        public const string ServiceName = "service.name";

        public const string BeginState = "masstransit.begin_state";
        public const string EndState = "masstransit.end_state";
        public const string SagaId = "masstransit.saga_id";


        public static class Messaging
        {
            public const string BodyLength = "messaging.message_payload_size_bytes";
            public const string ConversationId = "messaging.conversation_id";
            public const string Destination = "messaging.destination";
            public const string DestinationKind = "messaging.destination_kind";
            public const string MessageId = "messaging.message_id";
            public const string Operation = "messaging.operation";


            public static class RabbitMq
            {
                public const string RoutingKey = "messaging.rabbitmq.routing_key";
            }
        }
    }
}
