namespace MassTransit.Contracts
{
    public interface ExecuteActivityNodeInfo :
        InstanceInfo
    {
        ExecuteActivityInfo Activity { get; }
    }
}
