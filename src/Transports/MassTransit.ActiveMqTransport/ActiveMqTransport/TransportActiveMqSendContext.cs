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

        public MsgPriority? Priority { get; set; }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            Priority = ReadEnum<MsgPriority>(properties, PropertyNames.Priority);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            if (Priority != null)
                properties[PropertyNames.Priority] = Priority.ToString();
        }


        static class PropertyNames
        {
            public const string Priority = "AMQ-Priority";
        }
    }
}
