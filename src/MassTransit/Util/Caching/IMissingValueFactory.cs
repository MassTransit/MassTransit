namespace MassTransit.Util.Caching
{
    using System.Threading.Tasks;


    /// <summary>
    /// The missing value factory is used by default if not specified inline, in the case
    /// a value is not found in the index this will be used to create it.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface IMissingValueFactory<TValue>
        where TValue : class
    {
        /// <summary>
        /// Create the missing value
        /// </summary>
        /// <param name="key">The missing key</param>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <returns>The created value, once it becomes available</returns>
        Task<TValue> CreateMissingValue<TKey>(TKey key);
    }
}