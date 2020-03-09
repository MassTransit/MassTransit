namespace MassTransit.Contracts
{
    public interface IConsumerNodeInfo :
        InstanceInfo
    {
        ConsumerInfo Consumer { get; }
    }
}