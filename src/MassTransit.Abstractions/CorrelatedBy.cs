namespace MassTransit
{
    /// <summary>
    /// Used to identify a message as correlated so that the CorrelationId can be returned
    /// </summary>
    /// <typeparam name="TKey">The type of the CorrelationId used</typeparam>
    public interface CorrelatedBy<out TKey>
    {
        /// <summary>
        /// Returns the CorrelationId for the message
        /// </summary>
        TKey CorrelationId { get; }
    }
}
