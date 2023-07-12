namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Transports;


    public sealed class EventHubReceiveContext :
        BaseReceiveContext,
        EventHubConsumeContext
    {
        readonly ProcessEventArgs _eventArgs;
        readonly EventData _eventData;

        public EventHubReceiveContext(ProcessEventArgs eventArgs, ReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            _eventArgs = eventArgs;
            _eventData = eventArgs.Data;

            Body = new BytesMessageBody(eventArgs.Data.Body.ToArray());
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_eventData.Properties);

        public override MessageBody Body { get; }

        public DateTimeOffset EnqueuedTime => _eventData.EnqueuedTime;
        public long Offset => _eventData.Offset;
        public string PartitionId => _eventArgs.Partition.PartitionId;
        public string PartitionKey => _eventData.PartitionKey;
        public IDictionary<string, object> Properties => _eventData.Properties;
        public long SequenceNumber => _eventData.SequenceNumber;
        public IReadOnlyDictionary<string, object> SystemProperties => _eventData.SystemProperties;
    }
}
