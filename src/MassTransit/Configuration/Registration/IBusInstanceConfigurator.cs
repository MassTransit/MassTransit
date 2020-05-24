namespace MassTransit.Registration
{
    using GreenPipes;


    public interface IBusInstanceConfigurator :
        ISpecification
    {
        void Configure(IBusInstance busInstance);
    }
}
