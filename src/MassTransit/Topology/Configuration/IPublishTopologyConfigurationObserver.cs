namespace MassTransit.Topology
{
    public interface IPublishTopologyConfigurationObserver
    {
        void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
            where T : class;
    }
}
