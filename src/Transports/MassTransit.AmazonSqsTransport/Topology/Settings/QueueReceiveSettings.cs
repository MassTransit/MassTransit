namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using Configurators;


    public class QueueReceiveSettings :
        QueueSubscriptionConfigurator,
        ReceiveSettings
    {
        public QueueReceiveSettings(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
            PrefetchCount = Math.Min(Environment.ProcessorCount * 2, 10);
            WaitTimeSeconds = 1;
        }

        public int PrefetchCount { get; set; }

        public int WaitTimeSeconds { get; set; }

        public bool PurgeOnStartup { get; set; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
        }
    }
}
