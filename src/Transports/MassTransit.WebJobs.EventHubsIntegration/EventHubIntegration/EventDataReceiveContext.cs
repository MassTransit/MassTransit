namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.EventHubs;
    using Serialization;
    using Transports;


    public sealed class EventDataReceiveContext :
        BaseReceiveContext,
        EventDataContext
    {
        readonly EventData _eventData;

        public EventDataReceiveContext(EventData eventData, ReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            _eventData = eventData;

            Body = new MemoryMessageBody(_eventData.Body);
        }

        protected override IHeaderProvider HeaderProvider => new EventDataHeaderProvider(_eventData);

        public override MessageBody Body { get; }

        public DateTimeOffset EnqueuedTime => _eventData.EnqueuedTime;
        public long Offset => _eventData.Offset;
        public string PartitionKey => _eventData.PartitionKey;
        public IDictionary<string, object> Properties => _eventData.Properties;
        public long SequenceNumber => _eventData.SequenceNumber;
        public IReadOnlyDictionary<string, object> SystemProperties => _eventData.SystemProperties;
    }
}
