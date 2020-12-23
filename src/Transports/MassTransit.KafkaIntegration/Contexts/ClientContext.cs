namespace MassTransit.KafkaIntegration.Contexts
{
    using Confluent.Kafka;
    using GreenPipes;


    public interface ClientContext :
        PipeContext
    {
        ClientConfig Config { get; }
    }
}
