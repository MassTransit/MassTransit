namespace MassTransit.TestComponents.ForkJoint.Activities
{
    using Contracts;


    public interface GrillBurgerLog
    {
        BurgerPatty Patty { get; }
    }
}
