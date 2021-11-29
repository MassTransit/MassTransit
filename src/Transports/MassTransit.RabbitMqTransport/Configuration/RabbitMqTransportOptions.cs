namespace MassTransit
{
    using Metadata;


    public class RabbitMqTransportOptions
    {
        public RabbitMqTransportOptions()
        {
            Host = HostMetadataCache.IsRunningInContainer ? "rabbitmq" : "localhost";
            Port = 5672;
            VHost = "/";
            User = "guest";
            Pass = "guest";
        }

        public string Host { get; set; }
        public ushort Port { get; set; }
        public string VHost { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public bool UseSsl { get; set; }
    }
}
