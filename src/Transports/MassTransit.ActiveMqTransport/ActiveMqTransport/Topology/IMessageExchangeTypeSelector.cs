namespace MassTransit.ActiveMqTransport.Topology
{
    /// <summary>
    /// Used to select the exchange type for a published message
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessageExchangeTypeSelector<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// The default exchange type
        /// </summary>
        string DefaultExchangeType { get; }

        /// <summary>
        /// Returns the exchange type for the message type
        /// </summary>
        /// <param name="exchangeName">The exchange name</param>
        /// <returns>The exchange type for the send</returns>
        string GetExchangeType(string exchangeName);
    }
}
