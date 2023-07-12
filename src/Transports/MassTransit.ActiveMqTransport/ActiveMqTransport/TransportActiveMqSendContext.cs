namespace MassTransit.ActiveMqTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using Apache.NMS;
    using Context;


    public class TransportActiveMqSendContext<T> :
        MessageSendContext<T>,
        ActiveMqSendContext<T>
        where T : class
    {
        public TransportActiveMqSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
        }

        public string GroupId { get; set; }
        public int? GroupSequence { get; set; }
        public MsgPriority? Priority { get; set; }
        public IDestination ReplyDestination { get; set; }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            Priority = ReadEnum<MsgPriority>(properties, ActiveMqTransportPropertyNames.Priority);
            GroupId = ReadString(properties, ActiveMqTransportPropertyNames.GroupId);
            GroupSequence = ReadInt(properties, ActiveMqTransportPropertyNames.GroupSequence);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            if (Priority != null)
                properties[ActiveMqTransportPropertyNames.Priority] = Priority.ToString();
            if (GroupId != null)
                properties[ActiveMqTransportPropertyNames.GroupId] = GroupId;
            if (GroupSequence != null)
                properties[ActiveMqTransportPropertyNames.GroupSequence] = GroupSequence.Value;
        }
    }
}
