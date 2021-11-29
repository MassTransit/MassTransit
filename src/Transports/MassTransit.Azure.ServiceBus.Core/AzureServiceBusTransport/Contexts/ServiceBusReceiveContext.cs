namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using Azure.Messaging.ServiceBus;
    using Transports;


    public sealed class ServiceBusReceiveContext :
        BaseReceiveContext,
        ServiceBusMessageContext
    {
        readonly ServiceBusReceivedMessage _message;

        public ServiceBusReceiveContext(ServiceBusReceivedMessage message, ReceiveEndpointContext receiveEndpointContext)
            : base(message.DeliveryCount > 1, receiveEndpointContext)
        {
            _message = message;

            Body = new ServiceBusMessageBody(message.Body);
        }

        protected override IHeaderProvider HeaderProvider => new ServiceBusHeaderProvider(_message);

        public override MessageBody Body { get; }

        public string MessageId => _message.MessageId;

        public string CorrelationId => _message.CorrelationId;

        public TimeSpan TimeToLive => _message.TimeToLive;

        public DateTime ExpiresAt => _message.ExpiresAt.UtcDateTime;

        public IReadOnlyDictionary<string, object> Properties => _message.ApplicationProperties;

        public int DeliveryCount => _message.DeliveryCount;

        public string Label => _message.Subject;

        public long SequenceNumber => _message.SequenceNumber;

        public long EnqueuedSequenceNumber => _message.EnqueuedSequenceNumber;

        public string LockToken => _message.LockToken;

        public DateTime LockedUntil => _message.LockedUntil.UtcDateTime;

        public string SessionId => _message.SessionId;

        public long Size => _message.Body?.ToMemory().Length ?? 0;

        public string To => _message.To;

        public string ReplyToSessionId => _message.ReplyToSessionId;

        public string PartitionKey => _message.PartitionKey;

        public string ReplyTo => _message.ReplyTo;

        public DateTime EnqueuedTime => _message.EnqueuedTime.UtcDateTime;

        public DateTime ScheduledEnqueueTime => _message.ScheduledEnqueueTime.UtcDateTime;

        protected override ContentType GetContentType()
        {
            ContentType contentType = default;
            if (!string.IsNullOrWhiteSpace(_message.ContentType))
                contentType = ConvertToContentType(_message.ContentType);

            return contentType ?? base.GetContentType();
        }
    }
}
