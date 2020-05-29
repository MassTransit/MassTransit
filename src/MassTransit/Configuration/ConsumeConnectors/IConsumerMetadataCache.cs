namespace MassTransit.ConsumeConnectors
{
    public interface IConsumerMetadataCache<T>
    {
        IMessageInterfaceType[] ConsumerTypes { get; }
    }
}
