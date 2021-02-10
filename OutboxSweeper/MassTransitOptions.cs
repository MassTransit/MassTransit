namespace OutboxSweeper
{
    public class MassTransitOptions
    {
        public const string Key = "MassTransit";

        public RabbitMqSettings RabbitMq { get; set; }
        public AzureServiceBusSettings AzureServiceBus { get; set; }
    }

    public class RabbitMqSettings
    {
        public string HostAddress { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AzureServiceBusSettings
    {
        public string ConnectionString { get; set; }
    }
}
