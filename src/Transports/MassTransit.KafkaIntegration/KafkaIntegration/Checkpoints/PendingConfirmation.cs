namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Util;


    public class PendingConfirmation :
        IPendingConfirmation
    {
        readonly TaskCompletionSource<Offset> _source;

        public PendingConfirmation(TopicPartition partition, Offset offset)
        {
            Partition = partition;
            Offset = offset;
            _source = TaskUtil.GetTask<Offset>();
        }

        Uri Topic => new Uri($"topic:{Partition.Topic}");

        public TopicPartition Partition { get; }

        public Offset Offset { get; }

        public Task Confirmed => _source.Task;

        public void Complete()
        {
            _source.TrySetResult(Offset);
        }

        public void Faulted(Exception exception)
        {
            _source.TrySetException(new MessageNotConsumedException(Topic, "The message not confirmed", exception));
        }

        public void Faulted(string message)
        {
            _source.TrySetException(new ArgumentException(message));
        }
    }
}
