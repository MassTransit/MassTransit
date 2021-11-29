namespace MassTransit.Configuration
{
    /// <summary>
    /// Observes the configuration of message-specific topology
    /// </summary>
    public interface IMessageTopologyConfigurationObserver
    {
        void MessageTopologyCreated<T>(IMessageTopologyConfigurator<T> configuration)
            where T : class;
    }
}
