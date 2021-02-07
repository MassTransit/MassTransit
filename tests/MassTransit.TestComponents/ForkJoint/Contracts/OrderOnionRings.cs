namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    public interface OrderOnionRings :
        OrderLine
    {
        int Quantity { get; }
    }
}