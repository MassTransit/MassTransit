namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client;


    public class RabbitMqReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        readonly IRabbitMqEndpointConfiguration _configuration;

        public RabbitMqReceiveSettings(IRabbitMqEndpointConfiguration configuration, string name, string type, bool durable, bool autoDelete)
            : base(name, type, durable, autoDelete)
        {
            _configuration = configuration;

            ConsumeArguments = new Dictionary<string, object>();
        }

        public int ConsumerPriority
        {
            set => ConsumeArguments[Headers.XPriority] = value;
        }

        public ushort PrefetchCount
        {
            get => (ushort)_configuration.Transport.PrefetchCount;
            set => _configuration.Transport.Configurator.PrefetchCount = value;
        }

        public bool PurgeOnStartup { get; set; }
        public bool ExclusiveConsumer { get; set; }
        public bool NoAck { get; set; }

        public bool BindQueue { get; set; } = true;

        public IDictionary<string, object> ConsumeArguments { get; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
        }
    }
}
