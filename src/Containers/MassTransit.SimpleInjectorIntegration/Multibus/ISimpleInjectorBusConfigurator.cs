namespace MassTransit.SimpleInjectorIntegration.Multibus
{
    public interface ISimpleInjectorBusConfigurator<in TBus> :
        ISimpleInjectorBusConfigurator
        where TBus : class, IBus
    {
    }
}
