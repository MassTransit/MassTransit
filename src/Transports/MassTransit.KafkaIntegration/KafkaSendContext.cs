namespace MassTransit
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


    public interface KafkaSendContext<TKey, out T> :
        KafkaSendContext<T>
        where T : class
    {
        TKey Key { get; set; }
    }
}
