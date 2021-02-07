namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    public interface OrderFry :
        OrderLine
    {
        Size Size { get; }
    }
}