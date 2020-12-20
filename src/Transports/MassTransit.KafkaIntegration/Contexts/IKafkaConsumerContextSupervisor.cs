namespace MassTransit.KafkaIntegration.Contexts
{
    using Transport;
    using Transports;


    public interface IKafkaConsumerContextSupervisor<TKey, TValue> :
        ITransportSupervisor<IKafkaConsumerContext<TKey, TValue>>
        where TValue : class
    {
    }
}
