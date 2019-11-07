namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Context;
    using Microsoft.ServiceBus.Messaging;


    public sealed class ServiceBusReceiveContext :
        BaseReceiveContext,
        BrokeredMessageContext
    {
        readonly BrokeredMessage _message;
        byte[] _body;

        public ServiceBusReceiveContext(Uri inputAddress, BrokeredMessage message, ReceiveEndpointContext receiveEndpointContext)
            : base(inputAddress, message.DeliveryCount > 1, receiveEndpointContext)
        {
            _message = message;
        }

        protected override IHeaderProvider HeaderProvider => new ServiceBusHeaderProvider(this);

        public string MessageId => _message.MessageId;

        public string CorrelationId => _message.CorrelationId;

        public TimeSpan TimeToLive => _message.TimeToLive;

        public IDictionary<string, object> Properties => _message.Properties;

        public int DeliveryCount => _message.DeliveryCount;

        public string Label => _message.Label;

        public long SequenceNumber => _message.SequenceNumber;

        public long EnqueuedSequenceNumber => _message.EnqueuedSequenceNumber;

        public Guid LockToken => _message.LockToken;

        public DateTime LockedUntil => _message.LockedUntilUtc;

        public string SessionId => _message.SessionId;

        public long Size => _message.Size;

        public MessageState State => _message.State;

        public bool ForcePersistence => _message.ForcePersistence;

        public string To => _message.To;

        public string ReplyToSessionId => _message.ReplyToSessionId;

        public string PartitionKey => _message.PartitionKey;

        public string ViaPartitionKey => _message.ViaPartitionKey;

        public string ReplyTo => _message.ReplyTo;

        public DateTime EnqueuedTime => _message.EnqueuedTimeUtc;

        public DateTime ScheduledEnqueueTime => _message.ScheduledEnqueueTimeUtc;

        public Task RenewLockAsync()
        {
            return _message.RenewLockAsync();
        }

        public override byte[] GetBody()
        {
            if (_body == null)
                GetBodyAsByteArray();

            return _body;
        }

        public override Stream GetBodyStream()
        {
            if (_body == null)
                GetBodyAsByteArray();

            return new MemoryStream(_body, false);
        }

        void GetBodyAsByteArray()
        {
            using (var bodyStream = _message.GetBody<Stream>())
            using (var ms = new MemoryStream())
            {
                bodyStream.CopyTo(ms);

                _body = ms.ToArray();
            }
        }

        protected override ContentType GetContentType()
        {
            if (!string.IsNullOrWhiteSpace(_message.ContentType))
                return new ContentType(_message.ContentType);

            return base.GetContentType();
        }
    }
}
