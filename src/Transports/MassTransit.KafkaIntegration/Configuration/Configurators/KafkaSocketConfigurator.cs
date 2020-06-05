namespace MassTransit.KafkaIntegration.Configurators
{
    using System;
    using Confluent.Kafka;


    public class KafkaSocketConfigurator :
        IKafkaSocketConfigurator
    {
        readonly ClientConfig _clientConfig;

        public KafkaSocketConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public TimeSpan? Timeout
        {
            set => _clientConfig.SocketTimeoutMs = value?.Milliseconds;
        }

        public int? SendBufferBytes
        {
            set => _clientConfig.SocketSendBufferBytes = value;
        }

        public int? ReceiveBufferBytes
        {
            set => _clientConfig.SocketReceiveBufferBytes = value;
        }

        public bool? KeepaliveEnable
        {
            set => _clientConfig.SocketKeepaliveEnable = value;
        }

        public bool? NagleDisable
        {
            set => _clientConfig.SocketNagleDisable = value;
        }

        public int? MaxFails
        {
            set => _clientConfig.SocketMaxFails = value;
        }
    }
}
