namespace MassTransit.Topology
{
    /// <summary>
    /// Observes the configuration of message-specific topology
    /// </summary>
    public interface IMessageTopologyConfigurationObserver
    {
        void MessageTopologyCreated<T>(IMessageTopologyConfigurator<T> configuration)
            where T : class;

        void MessagePropertyTopologyCreated<TMessage, T>(IMessagePropertyTopologyConfigurator<TMessage, T> configuration)
            where TMessage : class;
    }
}
