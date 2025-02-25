namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Logging;


    public interface ConsumerContext :
        PipeContext
    {
        ILogContext LogContext { get; }
        IConsumer<byte[], byte[]> CreateConsumer(KafkaConsumerBuilderContext context, Action<IConsumer<byte[], byte[]>, Error> onError, int consumerIndex);
    }
}
