namespace MassTransit.Topology
{
    using GreenPipes;


    public interface ISendTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectSendTopologyConfigurationObserver(ISendTopologyConfigurationObserver observer);
    }
}
