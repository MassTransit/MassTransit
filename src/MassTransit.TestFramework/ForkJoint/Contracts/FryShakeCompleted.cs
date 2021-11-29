namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    public interface FryShakeCompleted :
        OrderLineCompleted
    {
        string Flavor { get; }
        Size Size { get; }
    }
}
