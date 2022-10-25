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

            Priority = ReadEnum<MsgPriority>(properties, PropertyNames.Priority);
            GroupId = ReadString(properties, PropertyNames.GroupId);
            GroupSequence = ReadInt(properties, PropertyNames.GroupSequence);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            if (Priority != null)
                properties[PropertyNames.Priority] = Priority.ToString();
            if (GroupId != null)
                properties[PropertyNames.GroupId] = GroupId;
            if (GroupSequence != null)
                properties[PropertyNames.GroupSequence] = GroupSequence.Value;
        }


        static class PropertyNames
        {
            public const string Priority = "AMQ-Priority";
            public const string GroupId = "AMQ-GroupId";
            public const string GroupSequence = "AMQ-GroupSequence";
        }
    }
}
