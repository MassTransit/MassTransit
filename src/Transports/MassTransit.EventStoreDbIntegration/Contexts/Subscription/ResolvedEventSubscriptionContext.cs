using System;
using System.IO;
using System.Threading.Tasks;
using EventStore.Client;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Transports;
using MassTransit.Util;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public sealed class ResolvedEventSubscriptionContext :
        BaseReceiveContext,
        ResolvedEventContext,
        ReceiveLockContext
    {
        readonly ResolvedEvent _resolvedEvent;
        readonly EventRecord _eventRecord;
        readonly ISubscriptionLockContext _lockContext;
        readonly IHeadersDeserializer _headersDeserializer;
        byte[] _body;
        byte[] _metadata;

        public ResolvedEventSubscriptionContext(
            ResolvedEvent resolvedEvent,
            ReceiveEndpointContext receiveEndpointContext,
            ISubscriptionLockContext lockContext,
            IHeadersDeserializer headersDeserializer)
            : base(false, receiveEndpointContext)
        {
            _resolvedEvent = resolvedEvent;
            _eventRecord = resolvedEvent.Event;
            _lockContext = lockContext;
            _headersDeserializer = headersDeserializer;
        }

        protected override IHeaderProvider HeaderProvider => _headersDeserializer.Deserialize(_resolvedEvent);

        public string EventStreamId => _eventRecord.EventStreamId;
        public string EventType => _eventRecord.EventType;
        public ulong? CommitPosition => _resolvedEvent.OriginalPosition?.CommitPosition;
        public ulong EventNumber => _resolvedEvent.Event.EventNumber.ToUInt64();
        public DateTime TimeStamp => _eventRecord.Created;
        public byte[] Metadata => _metadata ??= _eventRecord.Metadata.ToArray();

        public Task Complete() => _lockContext.Complete(_resolvedEvent);
        public Task Faulted(Exception exception) => TaskUtil.Completed;
        public Task ValidateLockStatus() => TaskUtil.Completed;

        public override byte[] GetBody() => _body ??= _eventRecord.Data.ToArray();
        public override Stream GetBodyStream() => new MemoryStream(GetBody(), false);
    }
}
