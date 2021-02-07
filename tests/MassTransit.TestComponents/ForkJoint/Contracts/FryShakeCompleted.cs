namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    public interface FryShakeCompleted :
        OrderLineCompleted
    {
        string Flavor { get; }
        Size Size { get; }
    }
}
