namespace MassTransit.Configuration
{
    public interface IMessageTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectMessageTopologyConfigurationObserver(IMessageTopologyConfigurationObserver observer);
    }
}
