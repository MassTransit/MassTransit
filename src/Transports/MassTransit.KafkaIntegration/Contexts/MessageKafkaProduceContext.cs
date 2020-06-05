namespace MassTransit.KafkaIntegration.Contexts
{
    using System.Threading;
    using Confluent.Kafka;
    using Context;


    public class MessageKafkaProduceContext<T> :
        MessageSendContext<T>,
        KafkaProduceContext<T>
        where T : class
    {
        public MessageKafkaProduceContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            Partition = Partition.Any;
        }

        public Partition Partition { get; set; }
    }
}
