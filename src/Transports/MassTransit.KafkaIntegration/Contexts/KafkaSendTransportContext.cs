namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Transport;


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
