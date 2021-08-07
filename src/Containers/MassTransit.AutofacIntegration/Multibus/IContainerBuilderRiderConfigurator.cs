namespace MassTransit.AutofacIntegration.MultiBus
{
    using Autofac;
    using MassTransit.Registration;


    public interface IContainerBuilderRiderConfigurator<in TBus> :
        IRiderRegistrationConfigurator
        where TBus : class, IBus
    {
        ContainerBuilder Builder { get; }
    }
}
