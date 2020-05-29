namespace MassTransit.RabbitMqTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using RabbitMQ.Client;


    public class RabbitMqReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        public RabbitMqReceiveSettings(string name, string type, bool durable, bool autoDelete)
            : base(name, type, durable, autoDelete)
        {
            PrefetchCount = (ushort)Math.Min(Environment.ProcessorCount * 2, 16);

            ConsumeArguments = new Dictionary<string, object>();
        }

        public int ConsumerPriority
        {
            set => ConsumeArguments[Headers.XPriority] = value;
        }

        public ushort PrefetchCount { get; set; }
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
