namespace MassTransit.ServiceBus
{
    /// <summary>
    /// The method used to dispatch the message to the service bus
    /// </summary>
    public enum DispatchMode
    {
        /// <summary>
        /// Dispatch the message in a synchronous fashion (default)
        /// </summary>
        Synchronous,

        /// <summary>
        /// Dipatch the message using an asynchronous handler
        /// </summary>
        Asynchronous,
    }
}