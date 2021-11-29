namespace MassTransit.Caching.Internals
{
    using System.Threading.Tasks;


    /// <summary>
    /// Holds a queue of pending values, attemping to resolve them in order until
    /// one of them completes, and then using the completing value for any pending
    /// values instead of calling their factory methods.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface INodeValueFactory<TValue>
        where TValue : class
    {
        /// <summary>
        /// Returns the final value of the factory, either completed or faulted
        /// </summary>
        Task<TValue> Value { get; }

        /// <summary>
        /// Add a pending value to the factory, which will either use a previously
        /// completed value or become the new factory method for the value.
        /// </summary>
        /// <param name="pendingValue">The factory method</param>
        void Add(IPendingValue<TValue> pendingValue);

        /// <summary>
        /// Called by the node tracker to create the value, which is then redistributed to the indices.
        /// Should not be called by another as it's used to resolve the value.
        /// </summary>
        /// <returns>The ultimate value task, either completed or faulted</returns>
        Task<TValue> CreateValue();
    }
}
