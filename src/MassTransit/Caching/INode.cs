namespace MassTransit.Caching
{
    using System.Threading.Tasks;


    public interface INode<TValue>
        where TValue : class
    {
        /// <summary>
        /// The cached value
        /// </summary>
        Task<TValue> Value { get; }

        /// <summary>
        /// True if the node has a value, resolved, ready to rock
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// True if the node value is invalid
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Get the node's value, passing a pending value if for some
        /// reason the node's value has not yet been accepted or has
        /// expired.
        /// </summary>
        /// <param name="pendingValue"></param>
        /// <returns></returns>
        Task<TValue> GetValue(IPendingValue<TValue> pendingValue);
    }
}
