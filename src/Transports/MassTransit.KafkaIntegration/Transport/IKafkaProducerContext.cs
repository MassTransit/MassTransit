namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Pipeline;
    using Pipeline.Observables;
    using Serializers;


    public interface IKafkaProducerContext<TKey, TValue> :
        ISendPipe,
        IDisposable
        where TValue : class
    {
        Uri HostAddress { get; }
        ILogContext LogContext { get; }
        SendObservable SendObservers { get; }

        IHeadersSerializer HeadersSerializer { get; }

        Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken);
    }
}
