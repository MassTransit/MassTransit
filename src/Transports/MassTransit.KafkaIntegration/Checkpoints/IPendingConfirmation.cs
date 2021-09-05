namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;


    public interface IPendingConfirmation
    {
        TopicPartition Partition { get; }
        Offset Offset { get; }

        Task Confirmed { get; }

        void Complete();
        void Faulted(Exception exception);
        void Faulted(string message);
    }
}
