namespace MassTransit
{
    using Configuration;
    using Confluent.Kafka;

    public class KafkaEndpointName : IEndpointName
    {
        public string Name { get; private set; }

        public KafkaEndpointName(string topicName, ConsumerConfig consumerConfig) : this(topicName, consumerConfig?.GroupId)
        {
        }

        public KafkaEndpointName(string topicName, string groupId)
        {
            Name = $"{KafkaTopicAddress.PathPrefix}/{topicName}";
            if (!string.IsNullOrWhiteSpace(groupId))
                Name = $"{Name}/{groupId}";
        }
    }
}
