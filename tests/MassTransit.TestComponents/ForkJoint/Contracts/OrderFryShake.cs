namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    public interface OrderFryShake :
        OrderLine
    {
        string Flavor { get; }
        Size Size { get; }
    }
}
