namespace MassTransit.KafkaIntegration.Transport
{
    using System;


    public interface IKafkaProducerFactory :
        IDisposable
    {
        Uri TopicAddress { get; }
    }
}
