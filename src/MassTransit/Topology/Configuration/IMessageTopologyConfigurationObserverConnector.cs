namespace MassTransit.Topology
{
    using GreenPipes;


    public interface IMessageTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectMessageTopologyConfigurationObserver(IMessageTopologyConfigurationObserver observer);
    }
}
