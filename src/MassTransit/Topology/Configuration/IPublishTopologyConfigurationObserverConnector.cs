namespace MassTransit.Topology
{
    using GreenPipes;


    public interface IPublishTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectPublishTopologyConfigurationObserver(IPublishTopologyConfigurationObserver observer);
    }
}
