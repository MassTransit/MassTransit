namespace MassTransit.Metadata
{
    using Contracts.Metadata;


    public interface IConsumerInfoCache<TConsumer> :
        IConsumerInfoCache
    {
    }


    public interface IConsumerInfoCache
    {
        ConsumerInfo ConsumerInfo { get; }
    }
}
