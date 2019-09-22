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
        public const string CorrelationConversationId = "correlation-conversation-id";

        public const string SourceHostMachine = "source-host-machine";
        public const string SourceHostProcessId = "source-host-process-id";
        public const string SourceHostFrameworkVersion = "source-host-framework-version";
        public const string SourceHostMassTransitVersion = "source-host-masstransit-version";
        public const string MessageTypes = "message-types";


        public static class StateMachine
        {
            public const string BeginState = "begin-state";
            public const string EndState = "end-state";
        }
    }
}
