namespace MassTransit.Registration
{
    using GreenPipes;


    public interface IBusInstanceSpecification :
        ISpecification
    {
        void Configure(IBusInstance busInstance);
    }
}
