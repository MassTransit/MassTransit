namespace MassTransit.ActiveMqTransport.Topology.Settings
{
    using System;
    using Configuration;
    using Configurators;


    public class QueueReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        readonly IActiveMqEndpointConfiguration _configuration;

        public QueueReceiveSettings(IActiveMqEndpointConfiguration configuration, string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
            _configuration = configuration;
        }

        public int PrefetchCount => _configuration.Transport.PrefetchCount;
        public int ConcurrentMessageLimit => _configuration.Transport.GetConcurrentMessageLimit();

        public Uri GetInputAddress(Uri hostAddress)
        {
            return GetEndpointAddress(hostAddress);
        }
    }
}
