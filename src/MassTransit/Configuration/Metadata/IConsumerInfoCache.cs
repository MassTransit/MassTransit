namespace MassTransit.Metadata
{
    using Contracts;


    public interface IConsumerInfoCache<TConsumer> :
        IConsumerInfoCache
    {
    }


    public interface IConsumerInfoCache
    {
        ConsumerInfo ConsumerInfo { get; }
    }
}
