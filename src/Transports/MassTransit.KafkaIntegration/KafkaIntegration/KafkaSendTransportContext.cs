namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Serializers;
    using Transports;


    public interface KafkaSendTransportContext<TKey, TValue> :
        SendTransportContext
        where TValue : class
    {
        Uri HostAddress { get; }
        KafkaTopicAddress TopicAddress { get; }
        ISendPipe SendPipe { get; }

        IHeadersSerializer HeadersSerializer { get; }
        IAsyncSerializer<TValue> ValueSerializer { get; }
        IAsyncSerializer<TKey> KeySerializer { get; }

        Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken);
    }
}
