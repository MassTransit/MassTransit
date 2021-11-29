namespace MassTransit.TestFramework.ForkJoint.Activities
{
    using Contracts;


    public interface GrillBurgerLog
    {
        BurgerPatty Patty { get; }
    }
}
