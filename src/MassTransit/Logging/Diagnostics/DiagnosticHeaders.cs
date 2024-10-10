#nullable enable
namespace MassTransit.Logging
{
    public static class DiagnosticHeaders
    {
        public const string DefaultListenerName = "MassTransit";

        public const string DiagnosticId = "Diagnostic-Id";
        public const string ActivityId = "MT-Activity-Id";
        public const string ActivityCorrelationContext = "MT-Activity-Correlation-Context";
        public const string ActivityPropagation = "MT-Activity-Propagation";

        public const string MessageId = "messaging.masstransit.message_id";
        public const string CorrelationId = "messaging.masstransit.correlation_id";
        public const string InitiatorId = "messaging.masstransit.initiator_id";
        public const string RequestId = "messaging.masstransit.request_id";
        public const string SourceAddress = "messaging.masstransit.source_address";
        public const string DestinationAddress = "messaging.masstransit.destination_address";
        public const string InputAddress = "messaging.masstransit.input_address";
        public const string TrackingNumber = "messaging.masstransit.tracking_number";

        public const string MessageTypes = "messaging.masstransit.message_types";

        public const string ConsumerType = "messaging.masstransit.consumer_type";

        public const string PeerAddress = "peer.address";

        public const string BeginState = "messaging.masstransit.begin_state";
        public const string EndState = "messaging.masstransit.end_state";
        public const string SagaId = "messaging.masstransit.saga_id";


        public class Exceptions
        {
            public const string EventName = "exception";
            public const string Type = "exception.type";
            public const string Message = "exception.message";
            public const string Escaped = "exception.escaped";
            public const string Stacktrace = "exception.stacktrace";
        }


        public static class Messaging
        {
            public const string BodyLength = "messaging.message.body.size";
            public const string ConversationId = "messaging.message.conversation_id";
            public const string DestinationName = "messaging.destination.name";
            public const string TransportMessageId = "messaging.message.id";
            public const string Operation = "messaging.operation";
            public const string System = "messaging.system";


            public static class RabbitMq
            {
                public const string RoutingKey = "messaging.rabbitmq.destination.routing_key";
            }
        }
    }
}
