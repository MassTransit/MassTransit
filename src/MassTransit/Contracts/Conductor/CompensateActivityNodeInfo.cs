namespace MassTransit.Contracts.Conductor
{
    using Metadata;


    public interface CompensateActivityNodeInfo :
        InstanceInfo
    {
        ActivityInfo Activity { get; }
    }
}
