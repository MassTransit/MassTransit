namespace MassTransit.Configuration
{
    public interface IConsumePipeSpecificationObserverConnector
    {
        ConnectHandle ConnectConsumePipeSpecificationObserver(IConsumePipeSpecificationObserver observer);
    }
}
