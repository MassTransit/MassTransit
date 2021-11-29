namespace MassTransit.Caching
{
    public interface IConnectCacheValueObserver<TValue>
        where TValue : class
    {
        ConnectHandle Connect(ICacheValueObserver<TValue> observer);
    }
}
