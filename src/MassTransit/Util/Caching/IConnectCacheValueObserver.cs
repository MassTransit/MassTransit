namespace MassTransit.Util.Caching
{
    using GreenPipes;


    public interface IConnectCacheValueObserver<TValue>
        where TValue : class
    {
        ConnectHandle Connect(ICacheValueObserver<TValue> observer);
    }
}