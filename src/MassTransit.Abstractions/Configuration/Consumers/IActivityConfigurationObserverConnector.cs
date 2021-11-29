namespace MassTransit
{
    public interface IActivityConfigurationObserverConnector
    {
        ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer);
    }
}
