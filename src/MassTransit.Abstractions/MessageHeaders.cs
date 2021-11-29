namespace MassTransit
{
    using Serialization;


    public static class MessageHeaders
    {
        /// <summary>
        /// The reason for a message action being taken
        /// </summary>
        public const string Reason = "MT-Reason";

        /// <summary>
        /// The type of exception from a Fault
        /// </summary>
        public const string FaultExceptionType = "MT-Fault-ExceptionType";

        /// <summary>
        /// The exception message from a Fault
        /// </summary>
        public const string FaultMessage = "MT-Fault-Message";

        /// <summary>
        /// The message type from a Fault
        /// </summary>
        public const string FaultMessageType = "MT-Fault-MessageType";

        /// <summary>
        /// The consumer type which faulted
        /// </summary>
        public const string FaultConsumerType = "MT-Fault-ConsumerType";

        /// <summary>
        /// The timestamp when the fault occurred
        /// </summary>
        public const string FaultTimestamp = "MT-Fault-Timestamp";

        /// <summary>
        /// The stack trace from a Fault
        /// </summary>
        public const string FaultStackTrace = "MT-Fault-StackTrace";

        /// <summary>
        /// The number of times the message was retried
        /// </summary>
        public const string FaultRetryCount = "MT-Fault-RetryCount";

        /// <summary>
        /// The number of times the message was redelivered
        /// </summary>
        public const string FaultRedeliveryCount = "MT-Fault-RedeliveryCount";

        /// <summary>
        /// The endpoint that forwarded the message to the new destination
        /// </summary>
        public const string ForwarderAddress = "MT-Forwarder-Address";

        /// <summary>
        /// The tokenId for the message that was registered with the scheduler
        /// </summary>
        public const string SchedulingTokenId = "MT-Scheduling-TokenId";

        /// <summary>
        /// The number of times the message has been redelivered (zero if never)
        /// </summary>
        public const string RedeliveryCount = "MT-Redelivery-Count";

        /// <summary>
        /// The trigger key that was used when the scheduled message was trigger
        /// </summary>
        public const string QuartzTriggerKey = "MT-Quartz-TriggerKey";

        /// <summary>
        /// Identifies the client from which the request is being sent
        /// </summary>
        public const string ClientId = "MT-Request-ClientId";

        /// <summary>
        /// Identifies the endpoint that handled the request
        /// </summary>
        public const string EndpointId = "MT-Request-EndpointId";

        /// <summary>
        /// The initiating conversation id if a new conversation was started by this message
        /// </summary>
        public const string InitiatingConversationId = "MT-InitiatingConversationId";

        /// <summary>
        /// MessageId - <see cref="MessageEnvelope" />
        /// </summary>
        public const string MessageId = "MessageId";

        /// <summary>
        /// CorrelationId - <see cref="MessageEnvelope" />
        /// </summary>
        public const string CorrelationId = "CorrelationId";

        /// <summary>
        /// ConversationId - <see cref="MessageEnvelope" />
        /// </summary>
        public const string ConversationId = "ConversationId";

        /// <summary>
        /// RequestId - <see cref="MessageEnvelope" />
        /// </summary>
        public const string RequestId = "RequestId";

        /// <summary>
        /// InitiatorId - <see cref="MessageEnvelope" />
        /// </summary>
        public const string InitiatorId = "MT-InitiatorId";

        /// <summary>
        /// SourceAddress - <see cref="MessageEnvelope" />
        /// </summary>
        public const string SourceAddress = "MT-Source-Address";

        /// <summary>
        /// ResponseAddress - <see cref="MessageEnvelope" />
        /// </summary>
        public const string ResponseAddress = "MT-Response-Address";

        /// <summary>
        /// FaultAddress - <see cref="MessageEnvelope" />
        /// </summary>
        public const string FaultAddress = "MT-Fault-Address";

        /// <summary>
        /// MessageType - <see cref="MessageEnvelope" />
        /// </summary>
        public const string MessageType = "MT-MessageType";

        /// <summary>
        /// The Transport message ID, which is a string, because we can't assume anything
        /// </summary>
        public const string TransportMessageId = "TransportMessageId";

        /// <summary>
        /// When the message is redelivered or scheduled, and a new MessageId was generated, the original messageId
        /// </summary>
        public const string OriginalMessageId = "MT-OriginalMessageId";

        /// <summary>
        /// When a transport header is used, this is the name
        /// </summary>
        public const string ContentType = "Content-Type";

        /// <summary>
        /// Used in routing slip variables to store the correlationId of a future
        /// </summary>
        public const string FutureId = "FutureId";


        public static class Host
        {
            public const string Info = "MT-Host-Info";
            public const string MachineName = "MT-Host-MachineName";
            public const string ProcessName = "MT-Host-ProcessName";
            public const string ProcessId = "MT-Host-ProcessId";
            public const string Assembly = "MT-Host-Assembly";
            public const string AssemblyVersion = "MT-Host-AssemblyVersion";
            public const string MassTransitVersion = "MT-Host-MassTransitVersion";
            public const string FrameworkVersion = "MT-Host-FrameworkVersion";
            public const string OperatingSystemVersion = "MT-Host-OperatingSystemVersion";
        }


        public static class Request
        {
            public const string Accept = "MT-Request-AcceptType";
        }


        public static class Quartz
        {
            /// <summary>
            /// The time when the message was scheduled
            /// </summary>
            public const string Scheduled = "MT-Quartz-Scheduled";

            /// <summary>
            /// When the event for this message was fired by Quartz
            /// </summary>
            public const string Sent = "MT-Quartz-Sent";

            /// <summary>
            /// When the next message is scheduled to be sent
            /// </summary>
            public const string NextScheduled = "MT-Quartz-NextScheduled";

            /// <summary>
            /// When the previous message was sent
            /// </summary>
            public const string PreviousSent = "MT-Quartz-PreviousSent";
        }
    }
}
