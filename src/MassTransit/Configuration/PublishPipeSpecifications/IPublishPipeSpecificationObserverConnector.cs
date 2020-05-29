namespace MassTransit.PublishPipeSpecifications
{
    using GreenPipes;


    public interface IPublishPipeSpecificationObserverConnector
    {
        ConnectHandle ConnectPublishPipeSpecificationObserver(IPublishPipeSpecificationObserver observer);
    }
}
