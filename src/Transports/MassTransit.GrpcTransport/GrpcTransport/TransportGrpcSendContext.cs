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
        }

        public string Exchange { get; private set; }
        public string RoutingKey { get; set; }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            Exchange = ReadString(properties, GrpcTransportPropertyNames.Exchange, Exchange);
            RoutingKey = ReadString(properties, GrpcTransportPropertyNames.RoutingKey);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            if (!string.IsNullOrWhiteSpace(Exchange))
                properties[GrpcTransportPropertyNames.Exchange] = Exchange;
            if (!string.IsNullOrWhiteSpace(RoutingKey))
                properties[GrpcTransportPropertyNames.RoutingKey] = RoutingKey;
        }
    }
}
