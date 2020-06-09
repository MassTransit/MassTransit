namespace MassTransit.KafkaIntegration
{
    using Confluent.Kafka;


    public interface KafkaSendContext :
        SendContext
    {
        Partition Partition { get; set; }
    }


    public interface KafkaSendContext<out T> :
        SendContext<T>,
        KafkaSendContext
        where T : class
    {
    }
}
