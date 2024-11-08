namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    public class RabbitMqMessageSendContext<T> :
        MessageSendContext<T>,
        RabbitMqSendContext<T>
        where T : class
    {
        public RabbitMqMessageSendContext(BasicProperties basicProperties, string exchange, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            BasicProperties = basicProperties;

            AwaitAck = true;

            RoutingKey = "";

            Exchange = exchange;
        }

        public string Exchange { get; private set; }
        public string RoutingKey { get; set; }
        public BasicProperties BasicProperties { get; }
        public bool AwaitAck { get; set; }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            Exchange = ReadString(properties, RabbitMqTransportPropertyNames.Exchange, Exchange);
            RoutingKey = ReadString(properties, RabbitMqTransportPropertyNames.RoutingKey, "");

            BasicProperties.AppId = ReadString(properties, RabbitMqTransportPropertyNames.AppId);
            BasicProperties.Priority = ReadByte(properties, RabbitMqTransportPropertyNames.Priority);
            BasicProperties.ReplyTo = ReadString(properties, RabbitMqTransportPropertyNames.ReplyTo);
            BasicProperties.Type = ReadString(properties, RabbitMqTransportPropertyNames.Type);
            BasicProperties.UserId = ReadString(properties, RabbitMqTransportPropertyNames.UserId);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            properties[RabbitMqTransportPropertyNames.Exchange] = Exchange;

            if (!string.IsNullOrWhiteSpace(RoutingKey))
                properties[RabbitMqTransportPropertyNames.RoutingKey] = RoutingKey;

            if (BasicProperties.IsAppIdPresent())
                properties[RabbitMqTransportPropertyNames.AppId] = BasicProperties.AppId;
            if (BasicProperties.IsPriorityPresent())
                properties[RabbitMqTransportPropertyNames.Priority] = BasicProperties.Priority;
            if (BasicProperties.IsReplyToPresent())
                properties[RabbitMqTransportPropertyNames.ReplyTo] = BasicProperties.ReplyTo;
            if (BasicProperties.IsTypePresent())
                properties[RabbitMqTransportPropertyNames.Type] = BasicProperties.Type;
            if (BasicProperties.IsUserIdPresent())
                properties[RabbitMqTransportPropertyNames.UserId] = BasicProperties.UserId;
        }
    }
}
