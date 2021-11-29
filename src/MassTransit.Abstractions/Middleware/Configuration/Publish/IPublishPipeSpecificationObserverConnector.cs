namespace MassTransit.Configuration
{
    public interface IPublishPipeSpecificationObserverConnector
    {
        ConnectHandle ConnectPublishPipeSpecificationObserver(IPublishPipeSpecificationObserver observer);
    }
}
