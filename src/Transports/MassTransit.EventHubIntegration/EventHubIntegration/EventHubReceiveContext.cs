namespace MassTransit.EventHubIntegration;

using System;
using System.Collections.Generic;
using System.Net.Mime;
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

    protected override IHeaderProvider HeaderProvider => new EventHubHeaderProvider(_eventData);

    public override MessageBody Body { get; }

    public DateTimeOffset EnqueuedTime => _eventData.EnqueuedTime;

    [Obsolete("The Event Hubs service does not guarantee a numeric offset for all resource configurations.  Please use 'OffsetString' instead.")]
    public long Offset => _eventData.Offset;

    public string OffsetString => _eventData.OffsetString;
    public string PartitionId => _eventArgs.Partition.PartitionId;
    public string PartitionKey => _eventData.PartitionKey;
    public IDictionary<string, object> Properties => _eventData.Properties;
    public long SequenceNumber => _eventData.SequenceNumber;
    public IReadOnlyDictionary<string, object> SystemProperties => _eventData.SystemProperties;

    protected override ContentType GetContentType()
    {
        ContentType contentType = default;
        if (!string.IsNullOrWhiteSpace(_eventData.ContentType))
            contentType = ConvertToContentType(_eventData.ContentType);

        return contentType ?? base.GetContentType();
    }
}
