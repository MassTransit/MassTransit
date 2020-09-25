namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using GreenPipes.Caching;


    /// <summary>
    /// Caches SendEndpoint instances by address (ignoring the query string entirely, case insensitive)
    /// </summary>
    public class SendEndpointCache<TKey> :
        ISendEndpointCache<TKey>
    {
        readonly IIndex<TKey, CachedSendEndpoint<TKey>> _index;

        public SendEndpointCache()
        {
            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);

            var cache = new GreenCache<CachedSendEndpoint<TKey>>(cacheSettings);
            _index = cache.AddIndex("key", x => x.Key);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(TKey key, SendEndpointFactory<TKey> factory)
        {
            CachedSendEndpoint<TKey> sendEndpoint = await _index.Get(key, x => GetSendEndpointFromFactory(x, factory)).ConfigureAwait(false);

            return sendEndpoint;
        }

        static async Task<CachedSendEndpoint<TKey>> GetSendEndpointFromFactory(TKey address, SendEndpointFactory<TKey> factory)
        {
            var sendEndpoint = await factory(address).ConfigureAwait(false);

            return new CachedSendEndpoint<TKey>(address, sendEndpoint);
        }
    }
}
