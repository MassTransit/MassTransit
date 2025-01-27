namespace MassTransit.KafkaIntegration
{
    using Confluent.Kafka;


    public delegate ConsumerBuilder<byte[], byte[]> ConsumerBuilderFactory(int index);
}
