namespace MassTransit
{
    using Confluent.Kafka;


    public interface KafkaSendContext :
        SendContext
    {
        Partition Partition { get; set; }
    }


    public interface KafkaSendContext<T> :
        SendContext<T>,
        KafkaSendContext
        where T : class
    {
        IAsyncSerializer<T> ValueSerializer { get; set; }
    }


    public interface KafkaSendContext<TKey, T> :
        KafkaSendContext<T>
        where T : class
    {
        TKey Key { get; set; }

        IAsyncSerializer<TKey> KeySerializer { get; set; }
    }
}
