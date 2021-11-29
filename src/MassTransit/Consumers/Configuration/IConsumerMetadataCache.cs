namespace MassTransit.Configuration
{
    public interface IConsumerMetadataCache<T>
    {
        IMessageInterfaceType[] ConsumerTypes { get; }
    }
}
