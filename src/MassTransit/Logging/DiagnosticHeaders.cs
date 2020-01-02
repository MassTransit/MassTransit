namespace MassTransit.Logging
{
    public static class DiagnosticHeaders
    {
        public const string DefaultListenerName = "MassTransit";

        public const string ActivityId = "MT-Activity-Id";
        public const string ActivityCorrelationContext = "MT-Activity-Correlation-Context";

        public const string MessageId = "message-id";
        public const string CorrelationId = "correlation-id";
        public const string InitiatorId = "initiator-id";
        public const string SourceAddress = "source-address";
        public const string DestinationAddress = "destination-address";
        public const string InputAddress = "input-address";
        public const string ConversationId = "conversation-id";
        public const string TrackingNumber = "tracking-number";

        public const string SourceHostMachine = "source-host-machine";
        public const string MessageTypes = "message-types";

        public const string ServiceKind = "span.kind";
        public const string PeerAddress = "peer.address";
        public const string PeerHost = "peer.host";
        public const string PeerService = "peer.service";

        public const string ConsumerType = "consumer-type";
        public const string MessageType = "message-type";
        public const string SagaType = "saga-type";
        public const string ActivityType = "activity-type";
        public const string ArgumentType = "argument-type";
        public const string LogType = "log-type";
        public const string BeginState = "begin-state";
        public const string EndState = "end-state";
        public const string SagaId = "saga-id";


        public static class Kind
        {
            public const string Producer = "producer";
            public const string Consumer = "consumer";
            public const string Client = "client";
            public const string Server = "server";
        }
    }
}
