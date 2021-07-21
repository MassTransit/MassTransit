namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using Configuration;
    using Configurators;


    public class QueueReceiveSettings :
        QueueSubscriptionConfigurator,
        ReceiveSettings
    {
        readonly IAmazonSqsEndpointConfiguration _configuration;

        public QueueReceiveSettings(IAmazonSqsEndpointConfiguration configuration, string queueName, bool durable, bool autoDelete, bool orderedMessageProcessingEnabled)
            : base(queueName, durable, autoDelete)
        {
            _configuration = configuration;
            OrderedMessageHandlingEnabled = orderedMessageProcessingEnabled;

            WaitTimeSeconds = 3;
        }

        public int PrefetchCount => _configuration.Transport.PrefetchCount;
        public int ConcurrentMessageLimit => _configuration.Transport.GetConcurrentMessageLimit();

        public int WaitTimeSeconds { get; set; }

        public bool PurgeOnStartup { get; set; }

        public bool OrderedMessageHandlingEnabled { get; set; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
        }
    }
}
