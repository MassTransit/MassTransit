namespace MassTransit.KafkaIntegration
{
    using Confluent.Kafka;


    public interface ClientContext :
        PipeContext
    {
        ClientConfig Config { get; }
    }
}
