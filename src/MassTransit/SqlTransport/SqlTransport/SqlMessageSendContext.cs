#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Context;


    public class SqlMessageSendContext<T> :
        MessageSendContext<T>,
        SqlSendContext<T>
        where T : class
    {
        public SqlMessageSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            TransportMessageId = NewId.NextGuid();
        }

        public Guid TransportMessageId { get; }

        public string? PartitionKey { get; set; }
        public short? Priority { get; set; }
        public string? RoutingKey { get; set; }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            PartitionKey = ReadString(properties, SqlTransportPropertyNames.PartitionKey);
            Priority = ReadShort(properties, SqlTransportPropertyNames.Priority);
            RoutingKey = ReadString(properties, SqlTransportPropertyNames.RoutingKey);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            if (!string.IsNullOrWhiteSpace(PartitionKey))
                properties[SqlTransportPropertyNames.PartitionKey] = PartitionKey!;
            if (Priority.HasValue)
                properties[SqlTransportPropertyNames.Priority] = Priority.Value;
            if (!string.IsNullOrWhiteSpace(RoutingKey))
                properties[SqlTransportPropertyNames.RoutingKey] = RoutingKey!;
        }
    }
}
