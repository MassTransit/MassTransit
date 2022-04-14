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
        public RabbitMqMessageSendContext(IBasicProperties basicProperties, string exchange, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            BasicProperties = basicProperties;

            AwaitAck = true;

            RoutingKey = "";

            Exchange = exchange;
        }

        public string Exchange { get; private set; }
        public string RoutingKey { get; set; }
        public IBasicProperties BasicProperties { get; }
        public bool AwaitAck { get; set; }

        public override void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
        {
            base.ReadPropertiesFrom(properties);

            Exchange = ReadString(properties, PropertyNames.Exchange, Exchange);
            RoutingKey = ReadString(properties, PropertyNames.RoutingKey, "");

            BasicProperties.AppId = ReadString(properties, PropertyNames.AppId);
            BasicProperties.Priority = ReadByte(properties, PropertyNames.Priority);
            BasicProperties.ReplyTo = ReadString(properties, PropertyNames.ReplyTo);
            BasicProperties.Type = ReadString(properties, PropertyNames.Type);
            BasicProperties.UserId = ReadString(properties, PropertyNames.UserId);
        }

        public override void WritePropertiesTo(IDictionary<string, object> properties)
        {
            base.WritePropertiesTo(properties);

            properties[PropertyNames.Exchange] = Exchange;

            if (!string.IsNullOrWhiteSpace(RoutingKey))
                properties[PropertyNames.RoutingKey] = RoutingKey;

            if (BasicProperties.IsAppIdPresent())
                properties[PropertyNames.AppId] = BasicProperties.AppId;
            if (BasicProperties.IsPriorityPresent())
                properties[PropertyNames.Priority] = BasicProperties.Priority;
            if (BasicProperties.IsReplyToPresent())
                properties[PropertyNames.ReplyTo] = BasicProperties.ReplyTo;
            if (BasicProperties.IsTypePresent())
                properties[PropertyNames.Type] = BasicProperties.Type;
            if (BasicProperties.IsUserIdPresent())
                properties[PropertyNames.UserId] = BasicProperties.UserId;
        }


        static class PropertyNames
        {
            public const string Exchange = "RMQ-Exchange";
            public const string RoutingKey = "RMQ-RoutingKey";

            public const string AppId = "RMQ-AppId";
            public const string Priority = "RMQ-Priority";
            public const string ReplyTo = "RMQ-ReplyTo";
            public const string Type = "RMQ-Type";
            public const string UserId = "RMQ-UserId";
        }
    }
}
