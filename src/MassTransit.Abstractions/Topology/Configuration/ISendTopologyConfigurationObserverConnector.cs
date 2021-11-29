namespace MassTransit.Configuration
{
    public interface ISendTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectSendTopologyConfigurationObserver(ISendTopologyConfigurationObserver observer);
    }
}
