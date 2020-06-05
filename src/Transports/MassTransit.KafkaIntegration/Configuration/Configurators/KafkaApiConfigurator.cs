namespace MassTransit.KafkaIntegration.Configurators
{
    using System;
    using Confluent.Kafka;


    public class KafkaApiConfigurator :
        IKafkaApiConfigurator
    {
        readonly ClientConfig _clientConfig;

        public KafkaApiConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public bool? Request
        {
            set => _clientConfig.ApiVersionRequest = value;
        }

        public TimeSpan? RequestTimeout
        {
            set => _clientConfig.ApiVersionRequestTimeoutMs = value?.Milliseconds;
        }

        public TimeSpan? FallbackTimeout
        {
            set => _clientConfig.ApiVersionFallbackMs = value?.Milliseconds;
        }

        public string BrokerVersionFallback
        {
            set => _clientConfig.BrokerVersionFallback = value;
        }
    }
}
