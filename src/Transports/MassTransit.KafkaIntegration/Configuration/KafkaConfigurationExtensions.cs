namespace MassTransit
{
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;


    public static class KafkaConfigurationExtensions
    {
        public static ConsumerConfig GetConsumerConfig(this IKafkaHostConfiguration hostConfiguration, ConsumerConfig config)
        {
            Dictionary<string, string> settings = hostConfiguration.Configuration.ToDictionary(setting => setting.Key, setting => setting.Value);

            foreach (KeyValuePair<string, string> setting in config)
                settings[setting.Key] = setting.Value;

            return new ConsumerConfig(settings);
        }

        public static ProducerConfig GetProducerConfig(this IKafkaHostConfiguration hostConfiguration, ProducerConfig config)
        {
            Dictionary<string, string> settings = hostConfiguration.Configuration.ToDictionary(setting => setting.Key, setting => setting.Value);

            foreach (KeyValuePair<string, string> setting in config)
                settings[setting.Key] = setting.Value;

            return new ProducerConfig(settings);
        }
    }
}
