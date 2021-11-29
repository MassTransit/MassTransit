namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


    public interface KafkaSendTransportContext<TKey, TValue> :
        SendTransportContext
        where TValue : class
    {
        Uri HostAddress { get; }
        KafkaTopicAddress TopicAddress { get; }
        ISendPipe SendPipe { get; }

        Task Send(IPipe<ProducerContext<TKey, TValue>> pipe, CancellationToken cancellationToken);
    }
}
