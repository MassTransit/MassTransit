namespace MassTransit.Configuration
{
    /// <summary>
    /// A convention cache for type specified, which converts to the generic type requested
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITopologyConventionCache<in T>
        where T : class
    {
        /// <summary>
        /// Returns the cached item for the specified type key, creating a new value
        /// if one has not yet been created.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        TResult GetOrAdd<TKey, TResult>()
            where TKey : class
            where TResult : class, T;
    }
}
