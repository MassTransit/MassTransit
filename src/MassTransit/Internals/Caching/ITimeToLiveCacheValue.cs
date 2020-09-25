namespace MassTransit.Internals.Caching
{
    public interface ITimeToLiveCacheValue<TValue> :
        ICacheValue<TValue>
        where TValue : class
    {
        long Timestamp { get; }
    }
}
