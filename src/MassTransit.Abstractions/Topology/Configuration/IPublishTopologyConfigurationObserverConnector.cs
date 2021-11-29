namespace MassTransit.Configuration
{
    public interface IPublishTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectPublishTopologyConfigurationObserver(IPublishTopologyConfigurationObserver observer);
    }
}
