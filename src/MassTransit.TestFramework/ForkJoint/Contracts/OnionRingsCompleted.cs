namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    public interface OnionRingsCompleted :
        OrderLineCompleted
    {
        int Quantity { get; }
    }
}
