namespace MassTransit.GrpcTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using Context;


    public class TransportGrpcSendContext<T> :
        MessageSendContext<T>,
        GrpcSendContext<T>
        where T : class
    {
        public TransportGrpcSendContext(string exchange, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            Exchange = exchange;
            RoutingKey = default;

            MessageTypes = MessageTypeCache<T>.MessageTypeNames;
        }

        public string Exchange { get; private set; }
        public string RoutingKey { get; set; }
        public string[] MessageTypes { get; set; }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            Exchange = ReadString(properties, GrpcTransportPropertyNames.Exchange, Exchange);
            MessageTypes = ReadStringArray(properties, GrpcTransportPropertyNames.MessageTypes);
            RoutingKey = ReadString(properties, GrpcTransportPropertyNames.RoutingKey);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            if (!string.IsNullOrWhiteSpace(Exchange))
                properties[GrpcTransportPropertyNames.Exchange] = Exchange;
            if (MessageTypes != null && MessageTypes.Length > 0)
                properties[GrpcTransportPropertyNames.MessageTypes] = string.Join(";", MessageTypes);
            if (!string.IsNullOrWhiteSpace(RoutingKey))
                properties[GrpcTransportPropertyNames.RoutingKey] = RoutingKey;
        }
    }
}
