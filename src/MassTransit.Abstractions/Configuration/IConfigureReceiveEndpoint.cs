namespace MassTransit
{
    /// <summary>
    /// Implement this interface, and register the implementation in the container as the interface
    /// type to apply configuration to all configured receive endpoints
    /// </summary>
    public interface IConfigureReceiveEndpoint
    {
        /// <summary>
        /// Configure the receive endpoint (called prior to any consumer, saga, or activity configuration)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurator"></param>
        void Configure(string name, IReceiveEndpointConfigurator configurator);
    }
}
