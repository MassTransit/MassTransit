namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    public interface OrderCombo :
        OrderLine
    {
        int Number { get; }
    }
}
