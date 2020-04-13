namespace MassTransit.Courier.Pipeline
{
    using GreenPipes;


    public interface IActivityObserverConnector
    {
        ConnectHandle ConnectActivityObserver(IActivityObserver observer);
    }
}
