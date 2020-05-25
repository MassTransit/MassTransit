namespace MassTransit.Contracts.Conductor
{
    using Metadata;


    public interface ActivityNodeInfo :
        InstanceInfo
    {
        ActivityInfo Activity { get; }
    }
}
