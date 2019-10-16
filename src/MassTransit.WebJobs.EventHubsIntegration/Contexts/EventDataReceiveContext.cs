namespace MassTransit.WebJobs.EventHubsIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Azure.ServiceBus.Core;
    using Context;
    using Microsoft.Azure.EventHubs;
    using Transports;


    public sealed class EventDataReceiveContext :
        BaseReceiveContext,
        EventDataContext
    {
        readonly EventData _eventData;
        byte[] _body;

        public EventDataReceiveContext(Uri inputAddress, EventData eventData, ReceiveEndpointContext receiveEndpointContext)
            : base(inputAddress, false, receiveEndpointContext)
        {
            _eventData = eventData;
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_eventData.Properties);

        public DateTime EnqueuedTime => _eventData.SystemProperties.EnqueuedTimeUtc;
        public string Offset => _eventData.SystemProperties.Offset;
        public string PartitionKey => _eventData.SystemProperties.PartitionKey;
        public IDictionary<string, object> Properties => _eventData.Properties;
        public long SequenceNumber => _eventData.SystemProperties.SequenceNumber;
        public IDictionary<string, object> SystemProperties => _eventData.SystemProperties;

        public override byte[] GetBody()
        {
            if (_body == null)
                _body = _eventData.Body.Array;

            return _body;
        }

        public override Stream GetBodyStream()
        {
            if (_body == null)
                _body = _eventData.Body.Array;

            return new MemoryStream(_body, false);
        }
    }
}
