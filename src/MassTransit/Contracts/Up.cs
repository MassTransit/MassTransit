namespace MassTransit.Contracts
{
    /// <summary>
    /// Announces that a service endpoint is up and available to accept that specified message type
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface Up<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// The service description, including the service address
        /// </summary>
        ServiceInfo Service { get; }

        /// <summary>
        /// The instance that produced the event
        /// </summary>
        InstanceInfo Instance { get; }

        /// <summary>
        /// The message details for the service endpoint
        /// </summary>
        MessageInfo Message { get; }
    }
}
