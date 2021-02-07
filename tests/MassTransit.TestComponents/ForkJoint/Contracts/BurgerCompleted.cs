namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    public interface BurgerCompleted :
        OrderLineCompleted
    {
        Burger Burger { get; }
    }
}