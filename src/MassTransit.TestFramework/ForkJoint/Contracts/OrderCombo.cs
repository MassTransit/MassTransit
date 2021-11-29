namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    public interface OrderCombo :
        OrderLine
    {
        int Number { get; }
    }
}
