namespace MassTransit.Contracts
{
    /// <summary>
    /// Announces that a service endpoint is down and no longer available to accept that specified message type
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface Down<TMessage>
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
    }
}
