#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Transports;


    public sealed class SqlReceiveContext :
        BaseReceiveContext,
        SqlMessageContext,
        TransportReceiveContext,
        ITransportSequenceNumber
    {
        IHeaderProvider? _headerProvider;

        public SqlReceiveContext(SqlTransportMessage message, SqlReceiveEndpointContext context, ReceiveSettings settings, ClientContext clientContext,
            ConnectionContext connectionContext, SqlReceiveLockContext lockContext)
            : base(message.DeliveryCount > 0, context, settings, clientContext, connectionContext, lockContext)
        {
            TransportMessage = message;

            Body = message.Body != null
                ? new StringMessageBody(message.Body)
                : new BytesMessageBody(message.BinaryBody);
        }

        public override MessageBody Body { get; }

        protected override IHeaderProvider HeaderProvider => _headerProvider ??= new SqlHeaderProvider(TransportMessage);

        public ulong? SequenceNumber => (ulong)DeliveryMessageId;

        public SqlTransportMessage TransportMessage { get; }

        public string? RoutingKey => TransportMessage.RoutingKey;
        public Guid TransportMessageId => TransportMessage.TransportMessageId;

        public Guid? ConsumerId => TransportMessage.ConsumerId;
        public Guid? LockId => TransportMessage.LockId;

        public string QueueName => TransportMessage.QueueName;
        public short Priority => TransportMessage.Priority;
        public long DeliveryMessageId => TransportMessage.MessageDeliveryId;
        public DateTime EnqueueTime => TransportMessage.EnqueueTime;
        public int DeliveryCount => TransportMessage.DeliveryCount;

        public string? PartitionKey => TransportMessage.PartitionKey;

        public IDictionary<string, object>? GetTransportProperties()
        {
            var properties = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>());

            if (!string.IsNullOrWhiteSpace(RoutingKey))
                properties.Value[SqlTransportPropertyNames.RoutingKey] = RoutingKey!;

            if (!string.IsNullOrWhiteSpace(PartitionKey))
                properties.Value[SqlTransportPropertyNames.PartitionKey] = PartitionKey!;

            if (Priority != 100)
                properties.Value[SqlTransportPropertyNames.Priority] = Priority;

            return properties.IsValueCreated ? properties.Value : null;
        }
    }
}
