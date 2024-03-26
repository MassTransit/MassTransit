namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using Internals.Caching;


    /// <summary>
    /// Caches SendEndpoint instances by address (ignoring the query string entirely, case insensitive)
    /// </summary>
    public class SendEndpointCache<TKey> :
        ISendEndpointCache<TKey>
    {
        readonly ICache<TKey, CachedSendEndpoint<TKey>, ITimeToLiveCacheValue<CachedSendEndpoint<TKey>>> _cache;

        public SendEndpointCache()
        {
            var options = new CacheOptions { Capacity = SendEndpointCacheDefaults.Capacity };
            var policy = new TimeToLiveCachePolicy<CachedSendEndpoint<TKey>>(SendEndpointCacheDefaults.MaxAge);

            _cache = new MassTransitCache<TKey, CachedSendEndpoint<TKey>, ITimeToLiveCacheValue<CachedSendEndpoint<TKey>>>(policy, options);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(TKey key, SendEndpointFactory<TKey> factory)
        {
            CachedSendEndpoint<TKey> sendEndpoint = await _cache.GetOrAdd(key, x => GetSendEndpointFromFactory(x, factory)).ConfigureAwait(false);

            return sendEndpoint;
        }

        static async Task<CachedSendEndpoint<TKey>> GetSendEndpointFromFactory(TKey address, SendEndpointFactory<TKey> factory)
        {
            var sendEndpoint = await factory(address).ConfigureAwait(false);

            return new CachedSendEndpoint<TKey>(address, sendEndpoint);
        }
    }
}
