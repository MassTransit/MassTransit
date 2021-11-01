namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;
    using Context;
    using global::Azure.Messaging.ServiceBus;


    public sealed class ServiceBusReceiveContext :
        BaseReceiveContext,
        ServiceBusMessageContext
    {
        readonly ServiceBusReceivedMessage _message;

        public ServiceBusReceiveContext(ServiceBusReceivedMessage message, ReceiveEndpointContext receiveEndpointContext)
            : base(message.DeliveryCount > 1, receiveEndpointContext)
        {
            _message = message;
        }

        protected override IHeaderProvider HeaderProvider => new ServiceBusHeaderProvider(_message);

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

        public override byte[] GetBody()
        {
            return _message.Body.ToArray();
        }

        public override Stream GetBodyStream()
        {
            return _message.Body.ToStream();
        }

        protected override ContentType GetContentType()
        {
            ContentType contentType = default;
            if (!string.IsNullOrWhiteSpace(_message.ContentType))
                contentType = ConvertToContentType(_message.ContentType);

            return contentType ?? base.GetContentType();
        }
    }
}
