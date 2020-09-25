namespace MassTransit.Internals.Caching
{
    using System.Threading.Tasks;


    public interface IPendingValue<TValue>
        where TValue : class
    {
        /// <summary>
        /// The value for the pending value
        /// </summary>
        Task<TValue> Value { get; }

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
