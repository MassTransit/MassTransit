namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Context;


    public class AmazonSqsMessageSendContext<T> :
        MessageSendContext<T>,
        AmazonSqsSendContext<T>
        where T : class
    {
        public AmazonSqsMessageSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
        }

        public string GroupId { get; set; }
        public string DeduplicationId { get; set; }

        public int? DelaySeconds
        {
            set => Delay = value.HasValue ? TimeSpan.FromSeconds(value.Value) : default;
        }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            GroupId = ReadString(properties, AmazonSqsTransportPropertyNames.GroupId);
            DeduplicationId = ReadString(properties, AmazonSqsTransportPropertyNames.DeduplicationId);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            if (!string.IsNullOrWhiteSpace(GroupId))
                properties[AmazonSqsTransportPropertyNames.GroupId] = GroupId;
            if (!string.IsNullOrWhiteSpace(DeduplicationId))
                properties[AmazonSqsTransportPropertyNames.DeduplicationId] = DeduplicationId;
        }
    }
}
