namespace MassTransit.Internals.Caching
{
    using System;
    using System.Threading.Tasks;


    public interface ICacheValue
    {
        bool HasValue { get; }

        /// <summary>
        /// True if the node value is invalid
        /// </summary>
        bool IsFaultedOrCanceled { get; }

        /// <summary>
        /// Tracks value usage
        /// </summary>
        int Usage { get; }

        /// <summary>
        /// Discard the value
        /// </summary>
        Task Evict();
    }


    public interface ICacheValue<TValue> :
        ICacheValue
        where TValue : class
    {
        Task<TValue> Value { get; }

        /// <summary>
        /// Get the node's value, passing a pending value if for some
        /// reason the node's value has not yet been accepted or has
        /// expired.
        /// </summary>
        /// <param name="pendingValueFactory"></param>
        /// <returns></returns>
        Task<TValue> GetValue(Func<IPendingValue<TValue>> pendingValueFactory);
    }
}
