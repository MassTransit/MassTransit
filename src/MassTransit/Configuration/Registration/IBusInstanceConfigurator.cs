namespace MassTransit.Registration
{
    public interface IBusInstanceConfigurator
    {
        void Configure(IBusInstance busInstance);
    }
}
