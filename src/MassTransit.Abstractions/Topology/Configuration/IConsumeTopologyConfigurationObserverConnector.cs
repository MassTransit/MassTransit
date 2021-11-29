namespace MassTransit.Configuration
{
    public interface IConsumeTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectConsumeTopologyConfigurationObserver(IConsumeTopologyConfigurationObserver observer);
    }
}
