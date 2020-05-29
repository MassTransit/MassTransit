namespace MassTransit.SendPipeSpecifications
{
    using GreenPipes;


    public interface ISendPipeSpecificationObserverConnector
    {
        ConnectHandle ConnectSendPipeSpecificationObserver(ISendPipeSpecificationObserver observer);
    }
}
