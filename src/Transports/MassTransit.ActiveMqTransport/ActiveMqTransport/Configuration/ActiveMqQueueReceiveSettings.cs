namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;


    public class ActiveMqQueueReceiveSettings :
        ActiveMqQueueBindingConfigurator,
        ReceiveSettings
    {
        readonly IActiveMqEndpointConfiguration _configuration;

        public ActiveMqQueueReceiveSettings(IActiveMqEndpointConfiguration configuration, string queueName, bool durable, bool autoDelete)
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
