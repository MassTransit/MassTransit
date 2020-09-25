namespace MassTransit.Internals.Caching
{
    using System.Threading.Tasks;


    public interface ICache<TKey, TValue, TCacheValue> :
        ICache<TKey, TValue>
        where TValue : class
        where TCacheValue : ICacheValue<TValue>
    {
    }


    public interface ICache<TKey, TValue>
        where TValue : class
    {
        int Count { get; }

        double HitRatio { get; }

        Task<TValue> GetOrAdd(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory);

        Task<TValue> Get(TKey key);
    }
}
