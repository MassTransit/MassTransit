namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    public interface OrderShake :
        OrderLine
    {
        string Flavor { get; }
        Size Size { get; }
    }
}
