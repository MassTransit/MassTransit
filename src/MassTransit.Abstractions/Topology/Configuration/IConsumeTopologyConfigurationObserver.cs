namespace MassTransit.Configuration
{
    public interface IConsumeTopologyConfigurationObserver
    {
        void MessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> configuration)
            where T : class;
    }
}
