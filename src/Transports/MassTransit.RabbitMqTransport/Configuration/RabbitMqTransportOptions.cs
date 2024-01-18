namespace MassTransit
{
    using Metadata;


    public class RabbitMqTransportOptions
    {
        const int DefaultPort = 5672;
        const int DefaultSslPort = 5671;
        const int DefaultManagementPort = 15672;
        const int DefaultSslManagementPort = 443;

        bool _useSsl;

        public RabbitMqTransportOptions()
        {
            Host = HostMetadataCache.IsRunningInContainer ? "rabbitmq" : "localhost";
            Port = DefaultPort;
            ManagementPort = DefaultManagementPort;
            VHost = "/";
            User = "guest";
            Pass = "guest";
        }

        public string Host { get; set; }
        public ushort Port { get; set; }
        public ushort ManagementPort { get; set; }
        public string VHost { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string ConnectionName { get; set; }

        public bool UseSsl
        {
            get => _useSsl;
            set
            {
                _useSsl = value;

                if (!_useSsl)
                    return;

                if (Port == DefaultPort)
                    Port = DefaultSslPort;

                if (ManagementPort == DefaultManagementPort)
                    ManagementPort = DefaultSslManagementPort;
            }
        }
    }
}
