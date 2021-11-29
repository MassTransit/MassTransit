namespace MassTransit.Caching
{
    using System.Threading.Tasks;


    /// <summary>
    /// An index is used to access items in the cache quickly
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IIndex<TKey, TValue>
        where TValue : class
    {
        Task<TValue> Get(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory = null);

        /// <summary>
        /// Forcibly removes the item from the cache, but disposal may occur asynchronously.
        /// </summary>
        /// <param name="key">The value key</param>
        bool Remove(TKey key);
    }
}
