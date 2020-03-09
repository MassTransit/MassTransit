namespace MassTransit.Contracts
{
    public interface ActivityNodeInfo :
        InstanceInfo
    {
        ActivityInfo Activity { get; }
    }
}
