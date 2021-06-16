namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;


    [Obsolete("Replace with IServiceCollectionBusConfigurator<TBus>. This interface will be removed in next versions.")]
    public interface IServiceCollectionConfigurator<in TBus> :
        IServiceCollectionBusConfigurator
        where TBus : class, IBus
    {
    }


    public interface IServiceCollectionBusConfigurator<in TBus> :
    #pragma warning disable 618
        IServiceCollectionConfigurator<TBus>
        where TBus : class, IBus
    {
    }
}
