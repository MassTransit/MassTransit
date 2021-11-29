namespace MassTransit.Configuration
{
    public interface ISendPipeSpecificationObserverConnector
    {
        ConnectHandle ConnectSendPipeSpecificationObserver(ISendPipeSpecificationObserver observer);
    }
}
