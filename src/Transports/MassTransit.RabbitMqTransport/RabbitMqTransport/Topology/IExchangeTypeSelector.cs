namespace MassTransit.RabbitMqTransport.Topology
{
    /// <summary>
    /// During a topology build, this will determine the exchange type for a message,
    /// given the exchange name (entity name) and routing key which have already been determined.
    /// </summary>
    public interface IExchangeTypeSelector
    {
        /// <summary>
        /// The default exchange type
        /// </summary>
        string DefaultExchangeType { get; }

        /// <summary>
        /// Returns the exchange type for the send context
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="exchangeName">The exchange name</param>
        /// <returns>The exchange type for the send</returns>
        string GetExchangeType<T>(string exchangeName)
            where T : class;
    }
}
