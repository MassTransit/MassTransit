namespace MassTransit.Topology
{
    using GreenPipes;


    public interface IConsumeTopologyConfigurationObserverConnector
    {
        ConnectHandle ConnectConsumeTopologyConfigurationObserver(IConsumeTopologyConfigurationObserver observer);
    }
}
