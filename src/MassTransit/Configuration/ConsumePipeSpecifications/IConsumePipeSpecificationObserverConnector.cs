namespace MassTransit.ConsumePipeSpecifications
{
    using GreenPipes;


    public interface IConsumePipeSpecificationObserverConnector
    {
        ConnectHandle ConnectConsumePipeSpecificationObserver(IConsumePipeSpecificationObserver observer);
    }
}
