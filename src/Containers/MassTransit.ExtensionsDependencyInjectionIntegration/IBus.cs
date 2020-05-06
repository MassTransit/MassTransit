namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    public interface IBus<in TBus> :
        IBus
        where TBus : class
    {
    }
}
