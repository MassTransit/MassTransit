namespace MassTransit.KafkaIntegration.Contexts
{
    using System.Threading;
    using Confluent.Kafka;
    using Context;


    public class KafkaMessageSendContext<T> :
        MessageSendContext<T>,
        KafkaSendContext<T>
        where T : class
    {
        public KafkaMessageSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            Partition = Partition.Any;
        }

        public Partition Partition { get; set; }
    }
}
