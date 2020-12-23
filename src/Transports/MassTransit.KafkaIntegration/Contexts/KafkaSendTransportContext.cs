namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using Context;
    using Pipeline;


    public interface KafkaSendTransportContext :
        SendTransportContext
    {
        Uri HostAddress { get; }
        KafkaTopicAddress TopicAddress { get; }
        ISendPipe SendPipe { get; }
    }
}
