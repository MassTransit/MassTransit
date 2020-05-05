namespace MassTransit.Topology
{
    public interface IConsumeTopologyConfigurationObserver
    {
        void MessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> configuration)
            where T : class;
    }
}
