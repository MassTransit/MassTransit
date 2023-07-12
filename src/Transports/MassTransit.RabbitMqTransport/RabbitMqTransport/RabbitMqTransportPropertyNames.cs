namespace MassTransit.RabbitMqTransport
{
    static class RabbitMqTransportPropertyNames
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
