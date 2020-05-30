namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Azure.Messaging.EventHubs;
    using Context;
    using Transports;


    public sealed class EventDataReceiveContext :
        BaseReceiveContext,
        EventDataContext
    {
        readonly EventData _eventData;
        byte[] _body;

        public EventDataReceiveContext(EventData eventData, ReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            _eventData = eventData;
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_eventData.Properties);

        public DateTimeOffset EnqueuedTime => _eventData.EnqueuedTime;
        public long Offset => _eventData.Offset;
        public string PartitionKey => _eventData.PartitionKey;
        public IDictionary<string, object> Properties => _eventData.Properties;
        public long SequenceNumber => _eventData.SequenceNumber;
        public IReadOnlyDictionary<string, object> SystemProperties => _eventData.SystemProperties;

        public override byte[] GetBody()
        {
            return _body ??= _eventData.Body.ToArray();
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(GetBody(), false);
        }
    }
}
