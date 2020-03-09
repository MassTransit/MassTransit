namespace MassTransit.Contracts
{
    public interface ICompensateActivityNodeInfo :
        InstanceInfo
    {
        ActivityInfo Activity { get; }
    }
}