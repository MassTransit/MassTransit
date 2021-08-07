namespace MassTransit.AutofacIntegration.Multibus
{
    public interface IContainerBuilderBusConfigurator<in TBus> :
        IContainerBuilderBusConfigurator
        where TBus : class, IBus
    {
    }
}
