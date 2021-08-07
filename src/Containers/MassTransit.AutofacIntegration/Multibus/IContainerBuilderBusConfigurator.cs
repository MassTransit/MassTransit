namespace MassTransit.AutofacIntegration.MultiBus
{
    public interface IContainerBuilderBusConfigurator<in TBus> :
        IContainerBuilderBusConfigurator
        where TBus : class, IBus
    {
    }
}
