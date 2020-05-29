namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public interface ISendEndpointCache<TKey>
    {
        /// <summary>
        /// Return a SendEndpoint from the cache, using the factory to create it if it doesn't exist in the cache.
        /// </summary>
        /// <param name="key">The key for the endpoint</param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<ISendEndpoint> GetSendEndpoint(TKey key, SendEndpointFactory<TKey> factory);
    }
}
