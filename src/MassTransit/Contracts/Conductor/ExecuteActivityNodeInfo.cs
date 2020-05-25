namespace MassTransit.Contracts.Conductor
{
    using Metadata;


    public interface ExecuteActivityNodeInfo :
        InstanceInfo
    {
        ExecuteActivityInfo Activity { get; }
    }
}
