namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using Confluent.Kafka;
    using Context;


    public class KafkaMessageSendContext<TKey, T> :
        MessageSendContext<T>,
        KafkaSendContext<TKey, T>
        where T : class
    {
        public KafkaMessageSendContext(TKey key, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            Key = key;
            Partition = Partition.Any;
        }

        public Partition Partition { get; set; }
        public TKey Key { get; set; }
    }
}
