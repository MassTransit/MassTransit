namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using Confluent.Kafka;


    public class KafkaFetchConfigurator :
        IKafkaFetchConfigurator
    {
        readonly ConsumerConfig _consumerConfig;

        public KafkaFetchConfigurator(ConsumerConfig consumerConfig)
        {
            _consumerConfig = consumerConfig;
        }

        public TimeSpan? WaitMaxInterval
        {
            set => _consumerConfig.FetchWaitMaxMs = (int)value?.TotalMilliseconds;
        }

        public int? MaxPartitionBytes
        {
            set => _consumerConfig.MaxPartitionFetchBytes = value;
        }

        public int? MaxBytes
        {
            set => _consumerConfig.FetchMaxBytes = value;
        }

        public int? MinBytes
        {
            set => _consumerConfig.FetchMinBytes = value;
        }

        public TimeSpan? ErrorBackoffInterval
        {
            set => _consumerConfig.FetchErrorBackoffMs = (int)value?.TotalMilliseconds;
        }
    }
}
