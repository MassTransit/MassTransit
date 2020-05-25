namespace MassTransit.Contracts.Conductor
{
    using Metadata;


    public interface ConsumerNodeInfo :
        InstanceInfo
    {
        ConsumerInfo Consumer { get; }
    }
}
