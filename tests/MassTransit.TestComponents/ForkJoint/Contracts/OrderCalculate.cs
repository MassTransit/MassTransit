namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    public interface OrderCalculate :
        OrderLine
    {
        int Number { get; }
    }
}
