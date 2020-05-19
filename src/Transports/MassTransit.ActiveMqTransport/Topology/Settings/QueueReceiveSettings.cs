namespace MassTransit.ActiveMqTransport.Topology.Settings
{
    using System;
    using Configurators;


    public class QueueReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        public QueueReceiveSettings(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
            PrefetchCount = (ushort)Math.Min(Environment.ProcessorCount * 2, 16);
        }

        public ushort PrefetchCount { get; set; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
        }
    }
}
