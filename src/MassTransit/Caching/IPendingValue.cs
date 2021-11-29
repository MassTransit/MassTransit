namespace MassTransit.Caching
{
    using System.Threading.Tasks;


    /// <summary>
    /// A pending Get on an index, which has yet to be processed. Used by the
    /// node value factory to sequentially resolve the value for an index item
    /// which is then added to the cache.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface IPendingValue<TValue>
        where TValue : class
    {
        /// <summary>
        /// Sets the pending value, eliminating the need for the factory method.
        /// </summary>
        /// <param name="value">The resolved value</param>
        void SetValue(Task<TValue> value);

        /// <summary>
        /// Create the value using the missing value factory supplied to Get
        /// </summary>
        /// <returns>Either the value, or a faulted task.</returns>
        Task<TValue> CreateValue();
    }
}
