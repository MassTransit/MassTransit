namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Transports;


    public sealed class EventHubReceiveContext :
        BaseReceiveContext,
        EventHubConsumeContext,
        ReceiveLockContext
    {
        readonly ProcessEventArgs _eventArgs;
        readonly EventData _eventData;
        readonly IProcessorLockContext _lockContext;

        public EventHubReceiveContext(ProcessEventArgs eventArgs, ReceiveEndpointContext receiveEndpointContext, IProcessorLockContext lockContext)
            : base(false, receiveEndpointContext)
        {
            _eventArgs = eventArgs;
            _eventData = eventArgs.Data;
            _lockContext = lockContext;

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

        public Task Complete()
        {
            return _lockContext.Complete(_eventArgs);
        }

        public Task Faulted(Exception exception)
        {
            return _lockContext.Faulted(_eventArgs, exception);
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }
    }
}
